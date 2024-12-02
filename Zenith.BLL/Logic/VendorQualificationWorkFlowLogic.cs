using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class VendorQualificationWorkFlowLogic : IVendorQualificationWorkFlow
    {
        private readonly IRepository<VendorQualificationWorkFlow> _VendorQualificationWorkFlowrepo;
        public readonly ZenithDbContext _zenithDbContext;

        public VendorQualificationWorkFlowLogic(IRepository<VendorQualificationWorkFlow> VendorQualificationWorkFlow,
            ZenithDbContext zenithDbContext)
        {
            _VendorQualificationWorkFlowrepo = VendorQualificationWorkFlow;
            _zenithDbContext = zenithDbContext;
        }

        public async Task<List<VendorQualificationWorkFlowDTO>> GetVendorQualificationWorkFlow(/*string VendorQualificationWorkFlowToUserId*/)
        {
           var rolesList= await _zenithDbContext.Roles.Where(x=>x.NormalizedName !=null).ToListAsync();
            var result = await (from a in _zenithDbContext.VendorQualificationWorkFlow
                                where a.IsActive
                                select new VendorQualificationWorkFlowDTO
                                {
                                    Id = a.Id,
                                    SecurityGroupId = a.SecurityGroupId,
                                    RoleId = a.RoleId,
                                    StepOrder = a.StepOrder,
                                    StepName = a.StepName,
                                    Description = a.Description,
                                    IsActive = a.IsActive,
                                    IsCriticalOnly = a.IsCriticalOnly,
                                    CreatedBy = a.CreatedBy,
                                    CreatedOn = a.CreatedOn,
                                }).ToListAsync();
            if (result!=null && result.Count>0 && rolesList.Count>0)
            {
                foreach (var item in result)
                {
                    var matchingRole = rolesList.Find(r => r.Id == Convert.ToString(item.RoleId));
                    if (matchingRole != null)
                    {
                        item.RoleName = matchingRole.Name??""; // Update RoleName based on matched role
                    }
                }
            }
            return result;
        }

        public async Task<Guid> AddVendorQualificationWorkFlow(VendorQualificationWorkFlowDTO model, string loggedInUserId)
        {
            VendorQualificationWorkFlow newRcrd=new VendorQualificationWorkFlow();
            if (model!=null)
            {
                newRcrd = new VendorQualificationWorkFlow()
                {
                    SecurityGroupId = model.SecurityGroupId,
                    RoleId = model.RoleId,
                    StepOrder = model.StepOrder,
                    StepName = model.StepName,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    IsCriticalOnly = model.IsCriticalOnly,
                    CreatedBy = loggedInUserId,
                    CreatedOn = DateTime.UtcNow,
                };
                 _VendorQualificationWorkFlowrepo.Add(newRcrd);
            }
            return newRcrd.Id;
        }

        public async Task<VendorQualificationWorkFlowDTO> GetVendorQualificationWorkFlowById(Guid vendorQualificationWorkFlowId)
        {
            var result = await (from a in _VendorQualificationWorkFlowrepo
                                where a.Id == vendorQualificationWorkFlowId
                                select new VendorQualificationWorkFlowDTO
                                {
                                    Id = a.Id,
                                    SecurityGroupId = a.SecurityGroupId,
                                    StepName = a.StepName,
                                    StepOrder = a.StepOrder,
                                    Description = a.Description,
                                    IsActive = a.IsActive,
                                    IsCriticalOnly = a.IsCriticalOnly,
                                    CreatedBy = a.CreatedBy,
                                    CreatedOn = a.CreatedOn,
                                    ModifiedBy = a.ModifiedBy,
                                    ModifiedOn = a.ModifiedOn,
                                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> UpdateVendorQualificationWorkFlow(VendorQualificationWorkFlowDTO model)
        {
            if (model != null)
            {
                var dbRcrd = await _VendorQualificationWorkFlowrepo.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (dbRcrd != null)
                {
                    dbRcrd.SecurityGroupId = model.SecurityGroupId;
                    dbRcrd.StepName = model.StepName;
                    dbRcrd.Description = model.Description;
                    dbRcrd.StepOrder = model.StepOrder;
                    dbRcrd.IsActive = model.IsActive;
                    dbRcrd.IsCriticalOnly = model.IsCriticalOnly;
                    dbRcrd.ModifiedBy = model.ModifiedBy;
                    dbRcrd.ModifiedOn = DateTime.Now;

                    await _VendorQualificationWorkFlowrepo.UpdateAsync(dbRcrd);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteVendorQualificationWorkFlow(Guid vendorQualificationWorkFlowId)
        {
            var dbRcrd = await _VendorQualificationWorkFlowrepo.Where(x => x.Id == vendorQualificationWorkFlowId).FirstOrDefaultAsync();
            if(dbRcrd == null)
                return false;

            await _VendorQualificationWorkFlowrepo.DeleteAsync(dbRcrd);
            return true;
        }
    }
}
