using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class VacationRequestsLogic : IVacationRequests
    {
        private readonly IRepository<VacationRequests> _vacationRequestsRepository;
        private readonly IDropdownList _IDropdownList;

        public VacationRequestsLogic(IRepository<VacationRequests> vacationRequestsRepository, IRepository<Address> AddressRepository,
            IRepository<Registrations> RegistrationRepository, IRepository<QualityCertification> QualityCertificationRepository,
            IRepository<AccountDetails> accountDetailRepository, IRepository<OtherDocuments> otherRepository,
            RoleManager<IdentityRole> roleManager, IDropdownList iDropdownList)
        {
            _vacationRequestsRepository = vacationRequestsRepository;
            _IDropdownList = iDropdownList;
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
                        }).ToListAsync();
            return data;
        }

        public async Task<List<VacationRequestsDTO>> GetWorkBenchVacationRequests(DateTime filterStartDate, DateTime filterEndDate)
        {
            var data = await (from a in _vacationRequestsRepository
                              where !a.IsDeleted  && a.CreatedOn.Date>= filterStartDate.Date && a.CreatedOn.Date<= filterEndDate.Date
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
                              }).ToListAsync();
            return data;
        }

        public async Task<VacationRequestsDTO> GetVacationRequestsId(int vacationRequestsId)
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
                          }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<int> AddVacationRequests(VacationRequestsDTO model, string loggedInUserId)
        {
            try
            {
                if (model != null)
                {
                    var pendingStatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.PENDING));
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
                        StatusId = pendingStatusId
                    };
                    _vacationRequestsRepository.Add(obj);
                    return obj.Id;
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
    }
}
