using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
using Zenith.Repository.RepositoryFiles;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Zenith.BLL.Logic
{
    public class VacationRequestsLogic : IVacationRequests
    {
        private readonly IRepository<VacationRequests> _vacationRequestsRepository;
        private readonly IDropdownList _IDropdownList;
        private readonly UserManager<ApplicationUser> _userManager;

        public VacationRequestsLogic(IRepository<VacationRequests> vacationRequestsRepository, IRepository<Address> AddressRepository,
            IRepository<Registrations> RegistrationRepository, IRepository<QualityCertification> QualityCertificationRepository,
            IRepository<AccountDetails> accountDetailRepository, IRepository<OtherDocuments> otherRepository,
            RoleManager<IdentityRole> roleManager, IDropdownList iDropdownList, UserManager<ApplicationUser> userManager)
        {
            _vacationRequestsRepository = vacationRequestsRepository;
            _IDropdownList = iDropdownList;
            _userManager = userManager;
        }

        public async Task<List<VacationRequestsDTO>> GetVacationRequests()
        {

            var data = await (from a in _vacationRequestsRepository
                        where !a.IsDeleted
                        select new VacationRequestsDTO
                        {
                            Id = a.Id,
                            IsApproved = a.IsApproved,
                            Comments = a.Comments,
                            IsActive = a.IsActive,
                            CreatedByUser = a.CreatedByUser,
                            RejectionReason = a.RejectionReason,
                            RequestedByUser = a.RequestedByUser,
                            CreatedOn = a.CreatedOn,
                            ModifiedBy = a.ModifiedBy,
                            ModifiedOn = a.ModifiedOn,
                            StartDate = a.StartDate,
                            EndDate = a.EndDate,
                            Status = a.Status,
                            RequestNum = a.RequestNum,
                            IsDelgateRequested = a.Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue(),
                        }).ToListAsync();
            return data;
        }

        public async Task<List<VacationRequestsDTO>> GetWorkBenchVacationRequests(DateTime filterStartDate, DateTime filterEndDate)
        {
            var data = await (from a in _vacationRequestsRepository
                              where !a.IsDeleted  && a.CreatedOn.Date>= filterStartDate.Date && a.CreatedOn.Date<= filterEndDate.Date && a.Status !=null 
                              && ( a.Status.Value== DropDownValuesEnum.PENDING.GetStringValue() || a.Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue())
                              select new VacationRequestsDTO
                              {
                                  Id = a.Id,
                                  IsApproved = a.IsApproved,
                                  Comments = a.Comments,
                                  IsActive = a.IsActive,
                                  CreatedByUser = a.CreatedByUser,
                                  RejectionReason = a.RejectionReason,
                                  RequestedByUser = a.RequestedByUser,
                                  CreatedOn = a.CreatedOn,
                                  ModifiedBy = a.ModifiedBy,
                                  ModifiedOn = a.ModifiedOn,
                                  StartDate = a.StartDate,
                                  EndDate = a.EndDate,
                                  Status = a.Status,
                                  RequestNum = a.RequestNum,
                                  IsDelgateRequested = a.Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue(),
                              }).ToListAsync();
            return data;
        }


        public async Task<List<VacationRequestsDTO>> GetAccountVacationRequests(string userId,DateTime filterStartDate, DateTime filterEndDate)
        {
            var data = await (from a in _vacationRequestsRepository
                              where !a.IsDeleted && a.Status != null && a.CreatedOn.Date >= filterStartDate.Date && a.CreatedOn.Date <= filterEndDate.Date && a.RequestedByUserId == userId
                              select new VacationRequestsDTO
                              {
                                  Id = a.Id,
                                  IsApproved = a.IsApproved,
                                  Comments = a.Comments,
                                  IsActive = a.IsActive,
                                  CreatedByUser = a.CreatedByUser,
                                  RejectionReason = a.RejectionReason,
                                  RequestedByUser = a.RequestedByUser,
                                  CreatedOn = a.CreatedOn,
                                  ModifiedBy = a.ModifiedBy,
                                  ModifiedOn = a.ModifiedOn,
                                  StartDate = a.StartDate,
                                  EndDate = a.EndDate,
                                  Status = a.Status,
                                  RequestNum = a.RequestNum,
                                  StatusText = (a.Status.Value == DropDownValuesEnum.APPROVED.GetStringValue() || a.Status.Value == DropDownValuesEnum.REJECTED.GetStringValue() )?
                                  a.Status.Value : DropDownValuesEnum.PENDING.GetStringValue(),
                                  ApproverId = a.ApproverId,
                                  Approver=a.Approver,
                              }).ToListAsync();
            return data;
        }

        public async Task<bool> CancelAllActiveVacationRequestsByUserId(string userId)
        {
            Guid cancldStatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.CANCELLED));
            var vacationRequestList = await _vacationRequestsRepository.Where(x=> !x.IsDeleted && x.IsActive && x.Status!=null && x.Status.Value==nameof(DropDownValuesEnum.PENDING)).ToListAsync();
            foreach (var item in vacationRequestList)
            {
                item.StatusId = cancldStatusId;
                item.ModifiedOn = DateTime.Now;
                await _vacationRequestsRepository.UpdateAsync(item);
            }
            await _vacationRequestsRepository.SaveChangesAsync();

            return true;
        }

        public async Task<VacationRequestsDTO> GetVacationRequestsId(Guid vacationRequestsId)
        {
            var result = await (from a in _vacationRequestsRepository
                          where a.Id == vacationRequestsId 
                                select new VacationRequestsDTO
                          {
                              Id = a.Id,
                              IsApproved = a.IsApproved,
                              Comments = a.Comments,
                              IsActive = a.IsActive,
                              CreatedByUser = a.CreatedByUser,
                              RejectionReason = a.RejectionReason,
                              RequestedByUser = a.RequestedByUser,
                              CreatedOn = a.CreatedOn,
                              ModifiedBy = a.ModifiedBy,
                              ModifiedOn = a.ModifiedOn,
                              StartDate = a.StartDate,
                              EndDate = a.EndDate,
                              Status = a.Status,
                              RequestNum = a.RequestNum,
                                }).FirstOrDefaultAsync();

            if (result != null) {
                var roleNames = await _userManager.GetRolesAsync(result.RequestedByUser);
                if (roleNames != null && roleNames.Count>0)
                    result.RequestedByUserRoleName = roleNames[0];
            }

            return result;
        }

        public async Task<int> AddVacationRequests(VacationRequestsDTO model, string loggedInUserId)
        {
            try
            {
                var user = _userManager.Users.Where(x => x.Id == loggedInUserId).FirstOrDefault();
                if (model != null && user!=null)
                {
                    var overlappingRequests = await _vacationRequestsRepository
                                                .Where(a => !a.IsDeleted &&
                                                    ((model.StartDate.Date >= a.StartDate.Date && model.StartDate.Date <= a.EndDate.Date) ||
                                                     (model.EndDate.Date >= a.StartDate.Date && model.EndDate.Date <= a.EndDate.Date) ||
                                                     (model.StartDate.Date <= a.StartDate.Date && model.EndDate.Date >= a.EndDate.Date)))
                                                .ToListAsync();

                    if (overlappingRequests.Any())
                    {
                        return -2; //There is an overlapping vacation request.
                    }
                    var pendingStatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.PENDING));
                      var maxSeriesNumber =  _vacationRequestsRepository.GetAll()
                               .Max(e => (int?)e.RequestNum) ?? 10000;
                    VacationRequests obj = new VacationRequests
                    {
                        Id = model.Id,
                        IsApproved = model.IsApproved,
                        Comments = model.Comments??"",
                        IsActive = model.IsActive,
                        RequestedByUserId = loggedInUserId,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        CreatedBy = loggedInUserId,
                        CreatedOn = DateTime.Now,
                        StatusId = pendingStatusId,
                        RequestNum = ++maxSeriesNumber,
                        ApproverId= user.ReportingManagerId
                    };
                    _vacationRequestsRepository.Add(obj);
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public async Task<string> UpdateVacationRequests(VacationRequestsDTO model)
        {
            var VCR = await _vacationRequestsRepository.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

            if (VCR != null)
            {
                if (model.IsApproved)
                {
                    VCR.IsApproved = true;
                    VCR.StatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.APPROVED));
                }
                else
                {
                    if (model.RejectionReasonId !=null && model.RejectionReasonId != Guid.Empty)
                    {
                        VCR.RejectionReasonId = model.RejectionReasonId;
                        VCR.StatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.REJECTED));
                    }
                }

                if (!string.IsNullOrEmpty(model.Comments))
                {
                    VCR.Comments = model.Comments;
                }

                await _vacationRequestsRepository.UpdateAsync(VCR);
                return "ok";
            }

            return "Something went wrong";
        }

        public async Task<bool> UpdateVacationRequestsStatuses(List<string> rcrdIds, string status)
        {
            if (rcrdIds == null || !rcrdIds.Any() || string.IsNullOrEmpty(status))
            {
                return false;
            }

            Guid statusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), status);
            if (statusId == Guid.Empty)
            {
                return false;
            }

            // Convert vendorIds to GUID and retrieve all vendors in a single query
            var rcrdGuidIds = rcrdIds.Select(id => new Guid(id)).ToList();
            var dbRecords = await _vacationRequestsRepository.Where(x => rcrdGuidIds.Any(y=>y==x.Id)).ToListAsync();

            if (!dbRecords.Any())
            {
                return false;
            }

            // Update the status of each vendor in memory
            foreach (var vendor in dbRecords)
            {
                vendor.StatusId = statusId;
            }

            // Save all changes at once
            await _vacationRequestsRepository.SaveChangesAsync();

            return true;
        }
    }
}
