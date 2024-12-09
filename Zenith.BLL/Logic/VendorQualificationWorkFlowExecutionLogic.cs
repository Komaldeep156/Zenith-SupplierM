using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class VendorQualificationWorkFlowExecutionLogic : IVendorQualificationWorkFlowExecution
    {
        private readonly IRepository<VendorQualificationWorkFlowExecution> _VendorQualificationWorkFlowExecutionrepo;
        public readonly ZenithDbContext _zenithDbContext;
        private readonly IDropdownList _iDropdownList;
        public VendorQualificationWorkFlowExecutionLogic(IRepository<VendorQualificationWorkFlowExecution> VendorQualificationWorkFlowExecution,
            ZenithDbContext zenithDbContext,
            IDropdownList iDropdownList)
        {
            _VendorQualificationWorkFlowExecutionrepo = VendorQualificationWorkFlowExecution;
            _zenithDbContext = zenithDbContext;
            _iDropdownList = iDropdownList;
        }

        public async Task<List<VendorQualificationWorkFlowExecutionDTO>> GetVendorQualificationWorkFlowExecution(string VendorQualificationWorkFlowExecutionToUserId)
        {
            var result = await (from a in _VendorQualificationWorkFlowExecutionrepo
                                where a.IsActive
                                select new VendorQualificationWorkFlowExecutionDTO
                                {
                                    AssignedUserId = a.AssignedUserId,
                                    VendorQualificationWorkFlowId = a.VendorQualificationWorkFlowId,
                                    IsActive = a.IsActive,
                                    VendorsInitializationFormId = a.VendorsInitializationFormId,
                                    StatusId = a.StatusId,
                                    CreatedBy = a.CreatedBy,
                                    CreatedOn = a.CreatedOn,
                                }).ToListAsync();

            return result;
        }

        public async Task<Guid> AddVendorQualificationWorkFlowExecution(VendorQualificationWorkFlowExecutionDTO model, string loggedInUserId)
        {
            VendorQualificationWorkFlowExecution newRcrd = new VendorQualificationWorkFlowExecution();
            if (model != null)
            {
                newRcrd = new VendorQualificationWorkFlowExecution()
                {
                    AssignedUserId = model.AssignedUserId,
                    VendorQualificationWorkFlowId = model.VendorQualificationWorkFlowId,
                    IsActive = model.IsActive,
                    VendorsInitializationFormId = model.VendorsInitializationFormId,
                    StatusId = model.StatusId,
                    CreatedBy = loggedInUserId,
                    CreatedOn = DateTime.UtcNow,
                };
                _VendorQualificationWorkFlowExecutionrepo.Add(newRcrd);
            }
            return newRcrd.Id;
        }

        public async Task<VendorQualificationWorkFlowExecutionDTO> GetVendorQualificationWorkFlowExecutionById(Guid VendorQualificationWorkFlowExecutionId)
        {
            var result = await (from a in _VendorQualificationWorkFlowExecutionrepo
                                where a.Id == VendorQualificationWorkFlowExecutionId
                                select new VendorQualificationWorkFlowExecutionDTO
                                {
                                    AssignedUserId = a.AssignedUserId,
                                    VendorQualificationWorkFlowId = a.VendorQualificationWorkFlowId,
                                    VendorsInitializationFormId = a.VendorsInitializationFormId,
                                    StatusId = a.StatusId,
                                    IsActive = a.IsActive,
                                    CreatedBy = a.CreatedBy,
                                    CreatedOn = a.CreatedOn,
                                    ModifiedBy = a.ModifiedBy,
                                    ModifiedOn = a.ModifiedOn,
                                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> UpdateVendorQualificationWorkFlowExecution(VendorQualificationWorkFlowExecutionDTO model)
        {
            if (model != null)
            {
                var dbRcrd = await _VendorQualificationWorkFlowExecutionrepo.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (dbRcrd != null)
                {
                    dbRcrd.AssignedUserId = model.AssignedUserId;
                    dbRcrd.VendorQualificationWorkFlowId = model.VendorQualificationWorkFlowId;
                    dbRcrd.VendorsInitializationFormId = model.VendorsInitializationFormId;
                    dbRcrd.StatusId = model.StatusId;
                    dbRcrd.IsActive = model.IsActive;
                    dbRcrd.ModifiedBy = model.ModifiedBy;
                    dbRcrd.ModifiedOn = DateTime.Now;

                    await _VendorQualificationWorkFlowExecutionrepo.UpdateAsync(dbRcrd);
                }
            }
            return true;
        }

        public async Task<bool> UpdateVendorQualificationWorkFlowExecutionStatusFromWorkBench(VendorQualificationWorkFlowExecutionDTO model)
        {
            if (model != null)
            {
                var dbRcrd = await _VendorQualificationWorkFlowExecutionrepo.Where(x => x.VendorsInitializationFormId == model.VendorsInitializationFormId && x.IsActive).FirstOrDefaultAsync();
                if (dbRcrd != null && model.StatusId != Guid.Empty)
                {
                    dbRcrd.StatusId = model.StatusId;
                    dbRcrd.ModifiedBy = model.ModifiedBy;
                    dbRcrd.ModifiedOn = DateTime.Now;

                    await _VendorQualificationWorkFlowExecutionrepo.UpdateAsync(dbRcrd);
                }
            }
            return true;
        }

        public async Task<bool> DelegateRequestedAssignVendorsToManager(DelegationRequests delegationRequests, string loggedInUserId)
        {
            if (delegationRequests == null ||
                string.IsNullOrEmpty(loggedInUserId) ||
                _iDropdownList == null)
            {
                return false;
            }

            Guid pendingStatusId = _iDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.PENDING));
            Guid delegatedStatusId = _iDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.DELEGATED));

            if (pendingStatusId == Guid.Empty || delegatedStatusId == Guid.Empty)
            {
                return false;
            }

            var workFlowExecution = await _zenithDbContext.VendorQualificationWorkFlowExecution
                .FirstOrDefaultAsync(x => x.VendorsInitializationFormId == delegationRequests.SourceId && x.IsActive);

            if (workFlowExecution == null)
            {
                return false;
            }

            workFlowExecution.IsActive = false;
            workFlowExecution.StatusId = delegatedStatusId;

            await _VendorQualificationWorkFlowExecutionrepo.UpdateAsync(workFlowExecution);

            var newWorkFlowExecution = new VendorQualificationWorkFlowExecutionDTO
            {
                AssignedUserId = delegationRequests.DelegateToUserId,
                VendorQualificationWorkFlowId = workFlowExecution.VendorQualificationWorkFlowId,
                IsActive = true,
                VendorsInitializationFormId = delegationRequests.SourceId,
                StatusId = pendingStatusId,
                CreatedBy = loggedInUserId,
                CreatedOn = DateTime.UtcNow,
            };

            _VendorQualificationWorkFlowExecutionrepo.Add(newWorkFlowExecution);

            return true;
        }

    }
}
