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
        private readonly IRepository<VacationRequests> _vacationRequests;
        private readonly IDropdownList _dropdownList;
        public readonly ZenithDbContext _zenithDbContext;
        private readonly IVendorQualificationWorkFlowExecution _vendorQualificationWorkFlowExecution;
        public DelegationRequestsLogic(IRepository<DelegationRequests> delegationRequests,
            IDropdownList dropdownList,
            ZenithDbContext zenithDbContext,
            IRepository<VacationRequests> vacationRequests,
            IVendorQualificationWorkFlowExecution vendorQualificationWorkFlowExecution)
        {
            _delegationRequests = delegationRequests;
            _dropdownList = dropdownList;
            _zenithDbContext = zenithDbContext;
            _vacationRequests = vacationRequests;
            _vendorQualificationWorkFlowExecution = vendorQualificationWorkFlowExecution;
        }

        /// <summary>
        /// Retrieves a list of delegation requests for a specific user.
        /// </summary>
        /// <param name="delegateToUserId">The ID of the user to whom the requests are delegated.</param>
        /// <returns>A list of <see cref="GetDelegateRequestDTO"/> objects.</returns>
        public async Task<List<GetDelegateRequestDTO>> GetDelegationRequests(string delegateToUserId)
        {
            var query = from d in _zenithDbContext.DelegationRequests
                        join v in _zenithDbContext.VendorsInitializationForm
                            on d.SourceId equals v.Id into vendorJoin
                        from vendor in vendorJoin.DefaultIfEmpty()

                        join vr in _zenithDbContext.VacationRequests
                            on d.SourceId equals vr.Id into vacationJoin
                        from vacation in vacationJoin.DefaultIfEmpty()

                        where d.Status != null && d.DelegateToUserId == delegateToUserId && d.Status.Value == DropDownValuesEnum.PENDING.GetStringValue()

                        select new GetDelegateRequestDTO
                        {
                            Id = d.Id,
                            SourceCd = GetSourceByApprovalType(d.ApprovalType),
                            ApprovalType = d.ApprovalType,
                            RequestNo = d.ApprovalType == ApprovalTypeEnum.VIR.GetStringValue() ? vendor.RequestNum.ToString() : d.ApprovalType == ApprovalTypeEnum.VACATION.GetStringValue() ? vacation.RequestNum.ToString() : "",
                            DelegateFromUser = d.DelegateFromUser,
                            DelegateToUser = d.DelegateToUser,
                            DelegatedRequestedOn = d.CreatedOn,
                            Status = d.Status,
                            SourceId = d.SourceId,
                        };

            var result = query.ToList();

            return result;
        }

        /// <summary>
        /// Gets the source code based on the approval type.
        /// </summary>
        /// <param name="approvalType">The approval type.</param>
        /// <returns>The source code as a string.</returns>
        public static string GetSourceByApprovalType(string approvalType)
        {
            string result = string.Empty;
            switch (approvalType)
            {
                case "VQR":
                    result = SourceCodesEnum.VENDOR.GetStringValue();
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

        /// <summary>
        /// Adds a new delegation request.
        /// </summary>
        /// <param name="model">The model containing the details of the delegation request.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public async Task<bool> AddNew(CreateDelegateRequestDTO model, string loggedInUserId)
        {
            List<string> rcrdIds = model.CommaSprtdRecordIds
                                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .ToList();

            if (!rcrdIds.Any())
            {
                return false;
            }

            var sourceIds = rcrdIds.Select(id => new Guid(id)).ToList();
            var pendingStatusId = _dropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.PENDING));
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

        /// <summary>
        /// Accepts or rejects a delegation request.
        /// </summary>
        /// <param name="delegateRequestId">The ID of the delegation request.</param>
        /// <param name="isDelegationReqAccepted">A boolean indicating whether the request is accepted.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public async Task<bool> AcceptOrRejectDelegateRequest(Guid delegateRequestId, bool isDelegationReqAccepted, string loggedInUserId)
        {
            Guid statusId;
            Guid pendingStatusId = _dropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.PENDING));
            Guid delegatedStatusId = _dropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.DELEGATED));

            if (delegateRequestId != Guid.Empty)
            {
                var dbRcrd = await _delegationRequests.Where(x => x.Id == delegateRequestId).FirstOrDefaultAsync();
                if (dbRcrd != null)
                {
                    if (isDelegationReqAccepted)
                        statusId = _dropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.ACCEPTED));
                    else
                        statusId = _dropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.DECLINED));

                    dbRcrd.StatusId = statusId;

                    if (dbRcrd.ApprovalType == ApprovalTypeEnum.VIR.GetStringValue())
                    {
                        //If Delegate request is reject then update the workstatus to delegated.
                        if (!isDelegationReqAccepted)
                        {
                            var workFlowExecution = await _zenithDbContext.VendorQualificationWorkFlowExecution
                                                .FirstOrDefaultAsync(x => x.VendorsInitializationFormId == dbRcrd.SourceId && x.IsActive);

                            if (workFlowExecution == null)
                                return false;

                            workFlowExecution.StatusId = pendingStatusId;
                        }

                        if (isDelegationReqAccepted)
                        {
                            if (!await _vendorQualificationWorkFlowExecution.DelegateRequestedAssignVendorsToManager(dbRcrd, loggedInUserId))
                                return false;
                        }
                    }
                    else if (dbRcrd.ApprovalType == ApprovalTypeEnum.VACATION.GetStringValue())
                    {
                        var vacationRecord = await _vacationRequests.Where(x => x.Id == dbRcrd.SourceId).FirstOrDefaultAsync();
                        if (vacationRecord != null)
                        {
                            //vacationRecord.StatusId = isDelegationReqAccepted ? delegatedStatusId : pendingStatusId;
                            vacationRecord.StatusId = pendingStatusId;
                            if (isDelegationReqAccepted)
                                vacationRecord.ApproverId = dbRcrd.DelegateToUserId;

                            await _vacationRequests.UpdateAsync(vacationRecord);
                        }
                    }

                    await _delegationRequests.UpdateAsync(dbRcrd);
                }
            }

            return true;
        }
    }
}
