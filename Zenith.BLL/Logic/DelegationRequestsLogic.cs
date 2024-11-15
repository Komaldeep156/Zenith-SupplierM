using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class DelegationRequestsLogic : IDelegationRequests
    {
        private readonly IRepository<DelegationRequests> _delegationRequests;
        private readonly IRepository<DropdownValues> _dropdownvalueRepository;
        private readonly IDropdownList _idropdownList;
        public readonly ZenithDbContext _zenithDbContext;
        public DelegationRequestsLogic(IRepository<DelegationRequests> delegationRequests, 
            IRepository<DropdownValues> dropdownvalueRepository, IDropdownList idropdownList, ZenithDbContext zenithDbContext)
        {
            _delegationRequests = delegationRequests;
            _dropdownvalueRepository = dropdownvalueRepository;
            _idropdownList = idropdownList;
            _zenithDbContext = zenithDbContext;
        }

        public async Task<List<GetDelegateRequestDTO>> GetDelegationRequests(string delegateToUserId)
        {
            var query = from d in _zenithDbContext.DelegationRequests
                        join v in _zenithDbContext.VendorsInitializationForm
                            on d.SourceId equals v.Id into vendorJoin
                        from vendor in vendorJoin.DefaultIfEmpty()

                        join vr in _zenithDbContext.VacationRequests
                            on d.SourceId equals vr.Id into vacationJoin
                        from vacation in vacationJoin.DefaultIfEmpty()

                        where d.Status!=null && d.Status.Value== DropDownValuesEnum.PENDING.GetStringValue()

                        select new GetDelegateRequestDTO
                        {
                            Id = d.Id,
                            SourceCd = GetSourceByApprovalType(d.ApprovalType),
                            ApprovalType = d.ApprovalType,
                            RequestNo = d.ApprovalType == ApprovalTypeEnum.VIR.GetStringValue() ? vendor.RequestNum.ToString() : d.ApprovalType == ApprovalTypeEnum.VACATION.GetStringValue() ? vacation.RequestNum.ToString() : "",
                            DelegateFromUser= d.DelegateFromUser,
                            DelegateToUser= d.DelegateToUser,
                            DelegatedRequestedOn= d.CreatedOn,
                            Status= d.Status,
                        };

            var result = query.ToList();

            return result;
        }

        public static string GetSourceByApprovalType(string approvalType)
        {
            string result = string.Empty;
            switch (approvalType)
            {
                case "VQR":
                    result=SourceCodesEnum.VENDOR.GetStringValue();
                    break;
                case "VIR":
                    result = SourceCodesEnum.VENDOR.GetStringValue();
                    break;
                case "VACATION":
                    result = SourceCodesEnum.USER.GetStringValue();
                    break;
                default:
                    break;
            }
            return result;
        }
            public async Task<bool> AddNew(CreateDelegateRequestDTO model, string loggedInUserId)
        {
            List<string> rcrdIds = model.CommaSprtdRecordIds
                                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .ToList();

            if (!rcrdIds.Any() )
            {
                return false;
            }

            var sourceIds = rcrdIds.Select(id => new Guid(id)).ToList();
            var pendingStatusId = _idropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.PENDING));
            List<DelegationRequests> delegationRequestsList = new List<DelegationRequests>();

            foreach (var item in sourceIds)
            {
                DelegationRequests newRcrd = new DelegationRequests()
                {
                    ApprovalType = model.RecordTypeCd,
                    SourceId = item,
                    DelegateFromUserId = loggedInUserId,
                    StatusId = pendingStatusId,
                    DelegateToUserId = model.DelegateToUserId,
                    CreatedBy = loggedInUserId,
                    CreatedOn = DateTime.UtcNow,
                };

                delegationRequestsList.Add(newRcrd);
            }

            _delegationRequests.AddRange(delegationRequestsList);

            return true;
        }
        
    }
}
