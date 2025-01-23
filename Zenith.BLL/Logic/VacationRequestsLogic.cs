using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
using Zenith.Repository.RepositoryFiles;

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

        /// <summary>
        /// Retrieves a list of vacation requests based on the assigned user ID.
        /// </summary>
        /// <param name="assignedUserId">The ID of the assigned user.</param>
        /// <returns>A list of vacation requests.</returns>
        public async Task<List<VacationRequestsDTO>> GetVacationRequests(string assignedUserId = default)
        {

            var data = await (from a in _vacationRequestsRepository
                              where !a.IsDeleted && (assignedUserId != null || a.ApproverId == assignedUserId)
                              && (a.Status.Value == DropDownValuesEnum.PENDING.GetStringValue() || a.Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue())
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
                                  StatusId = a.StatusId,
                                  RequestNum = a.RequestNum,
                                  IsDelgateRequested = a.Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue(),
                              }).ToListAsync();
            return data;
        }

        /// <summary>
        /// Retrieves a list of vacation requests for the workbench within a date range and for a specific user.
        /// </summary>
        /// <param name="filterStartDate">The start date for filtering vacation requests.</param>
        /// <param name="filterEndDate">The end date for filtering vacation requests.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A list of vacation requests for the workbench.</returns>
        public async Task<List<VacationRequestsDTO>> GetWorkBenchVacationRequests(DateTime filterStartDate, DateTime filterEndDate, string loggedInUserId = default)
        {
            var data = await (from a in _vacationRequestsRepository
                              where !a.IsDeleted && a.CreatedOn.Date >= filterStartDate.Date && a.CreatedOn.Date <= filterEndDate.Date && a.Status != null && a.ApproverId == loggedInUserId
                              && (a.Status.Value == DropDownValuesEnum.PENDING.GetStringValue() || a.Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue() || a.Status.Value == DropDownValuesEnum.WORKING.GetStringValue())
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
                                  StatusId = a.StatusId,
                                  IsDelgateRequested = a.Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue(),
                              }).ToListAsync();
            return data;
        }

        /// <summary>
        /// Retrieves a list of vacation requests for a specific user within a date range.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="filterStartDate">The start date for filtering vacation requests.</param>
        /// <param name="filterEndDate">The end date for filtering vacation requests.</param>
        /// <returns>A list of vacation requests for the user.</returns>
        public async Task<List<VacationRequestsDTO>> GetAccountVacationRequests(string userId, DateTime filterStartDate, DateTime filterEndDate)
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
                                  StatusText = (a.Status.Value == DropDownValuesEnum.APPROVED.GetStringValue() || a.Status.Value == DropDownValuesEnum.CANCELLED.GetStringValue() || a.Status.Value == DropDownValuesEnum.REJECTED.GetStringValue()) ?
                                  a.Status.Value : DropDownValuesEnum.PENDING.GetStringValue(),
                                  ApproverId = a.ApproverId,
                                  Approver = a.Approver,
                              }).ToListAsync();
            return data;
        }

        /// <summary>
        /// Cancels all active vacation requests for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public async Task<bool> CancelAllActiveVacationRequestsByUserId(string userId)
        {
            Guid cancldStatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.CANCELLED));
            Guid rejectReasonId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.REJECTREASON), nameof(DropDownValuesEnum.INAUSR));
            var vacationRequestList = await _vacationRequestsRepository.Where(x => x.RequestedByUserId == userId && !x.IsDeleted && x.Status != null && x.Status.Value == nameof(DropDownValuesEnum.PENDING)).ToListAsync();
            foreach (var item in vacationRequestList)
            {
                item.StatusId = cancldStatusId;
                item.ModifiedOn = DateTime.Now;
                item.RejectionReasonId = rejectReasonId;
                await _vacationRequestsRepository.UpdateAsync(item);
            }
            await _vacationRequestsRepository.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Retrieves a vacation request by its ID.
        /// </summary>
        /// <param name="vacationRequestsId">The ID of the vacation request.</param>
        /// <returns>A vacation request DTO.</returns>
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

            if (result != null)
            {
                var roleNames = await _userManager.GetRolesAsync(result.RequestedByUser);
                if (roleNames != null && roleNames.Count > 0)
                    result.RequestedByUserRoleName = roleNames[0];
            }

            return result;
        }

        /// <summary>
        /// Adds a new vacation request.
        /// </summary>
        /// <param name="model">The vacation request DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        public async Task<int> AddVacationRequests(VacationRequestsDTO model, string loggedInUserId)
        {
            try
            {
                var user = _userManager.Users.Where(x => x.Id == loggedInUserId).FirstOrDefault();
                if (model != null && user != null)
                {
                    var overlappingRequests = await _vacationRequestsRepository
                                                .Where(a => a.RequestedByUserId == loggedInUserId && !a.IsDeleted &&
                                                    ((model.StartDate.Date >= a.StartDate.Date && model.StartDate.Date <= a.EndDate.Date) ||
                                                     (model.EndDate.Date >= a.StartDate.Date && model.EndDate.Date <= a.EndDate.Date) ||
                                                     (model.StartDate.Date <= a.StartDate.Date && model.EndDate.Date >= a.EndDate.Date)))
                                                .ToListAsync();

                    if (overlappingRequests.Any())
                    {
                        return -2; //There is an overlapping vacation request.
                    }
                    var pendingStatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.PENDING));
                    var maxSeriesNumber = _vacationRequestsRepository.GetAll()
                             .Max(e => (int?)e.RequestNum) ?? 10000;
                    VacationRequests obj = new VacationRequests
                    {
                        Id = model.Id,
                        IsApproved = model.IsApproved,
                        Comments = model.Comments ?? "",
                        IsActive = model.IsActive,
                        RequestedByUserId = loggedInUserId,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        CreatedBy = loggedInUserId,
                        CreatedOn = DateTime.Now,
                        StatusId = pendingStatusId,
                        RequestNum = ++maxSeriesNumber,
                        ApproverId = user.ReportingManagerId
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

        /// <summary>
        /// Updates an existing vacation request.
        /// </summary>
        /// <param name="model">The vacation request DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
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
                    if (model.RejectionReasonId != null && model.RejectionReasonId != Guid.Empty)
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

        /// <summary>
        /// Updates the statuses of multiple vacation requests.
        /// </summary>
        /// <param name="rcrdIds">A list of record IDs.</param>
        /// <param name="status">The new status.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
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
            var dbRecords = await _vacationRequestsRepository.Where(x => rcrdGuidIds.Any(y => y == x.Id)).ToListAsync();

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

        /// <summary>
        /// Updates the status of a specific vacation request.
        /// </summary>
        /// <param name="model">The vacation request model.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public async Task<bool> UpdateVacationRequestStatus(VacationRequests model)
        {
            if (model != null)
            {
                var dbRcrd = await _vacationRequestsRepository.Where(x =>x.Id == model.Id).FirstOrDefaultAsync();
                if (dbRcrd != null && model.StatusId != Guid.Empty)
                {
                    dbRcrd.StatusId = model.StatusId;
                    dbRcrd.ModifiedBy = model.ModifiedBy;
                    dbRcrd.ModifiedOn = DateTime.Now;

                    _vacationRequestsRepository.Update(dbRcrd);
                }
            }
            return true;
        }
    }
}
