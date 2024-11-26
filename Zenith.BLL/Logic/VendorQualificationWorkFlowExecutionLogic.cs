using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class VendorQualificationWorkFlowExecutionLogic : IVendorQualificationWorkFlowExecution
    {
        private readonly IRepository<VendorQualificationWorkFlowExecution> _VendorQualificationWorkFlowExecutionrepo;
        public readonly ZenithDbContext _zenithDbContext;
        public VendorQualificationWorkFlowExecutionLogic(IRepository<VendorQualificationWorkFlowExecution> VendorQualificationWorkFlowExecution, 
            ZenithDbContext zenithDbContext
            )
        {
            _VendorQualificationWorkFlowExecutionrepo = VendorQualificationWorkFlowExecution;
            _zenithDbContext = zenithDbContext;
        }

        public async Task<List<VendorQualificationWorkFlowExecutionDTO>> GetVendorQualificationWorkFlowExecution(string VendorQualificationWorkFlowExecutionToUserId)
        {
            var result = await (from a in _VendorQualificationWorkFlowExecutionrepo
                                where a.IsActive
                                select new VendorQualificationWorkFlowExecutionDTO
                                {
                                    AssignedUserId = a.AssignedUserId,
                                    VendorQualificationWorkFlowId=a.VendorQualificationWorkFlowId,
                                    IsActive = a.IsActive,
                                    VendorsInitializationFormId=a.VendorsInitializationFormId,
                                    StatusId=a.StatusId,
                                    CreatedBy = a.CreatedBy,
                                    CreatedOn = a.CreatedOn,
                                }).ToListAsync();

            return result;
        }

        public async Task<Guid> AddVendorQualificationWorkFlowExecution(VendorQualificationWorkFlowExecutionDTO model, string loggedInUserId)
        {
            VendorQualificationWorkFlowExecution newRcrd=new VendorQualificationWorkFlowExecution();
            if (model!=null)
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
    }
}
