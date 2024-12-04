using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class WorkFlowsLogic : IWorkFlows
    {
        public readonly ZenithDbContext _zenithDbContext;
        private readonly IRepository<WorkFlows> _workFlowRepository;

        public WorkFlowsLogic(ZenithDbContext zenithDbContext,
            IRepository<WorkFlows> workFlowRepository)
        {
            _zenithDbContext = zenithDbContext;
            _workFlowRepository = workFlowRepository;
        }

        public async Task<List<WorkFlowsDTO>> GetWorkFlows()
        {
            var workFlowList = await _zenithDbContext.WorkFlows
                .Select(a => new WorkFlowsDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsActive = a.IsActive,
                    IsCritical = a.IsCritical,
                    Description = a.Description
                })
                .ToListAsync();

            return workFlowList;
        }

        public async Task<bool> DeleteWorkFlows(List<Guid> selectedWorkFlowsIds)
        {
            if (selectedWorkFlowsIds != null)
            {
                foreach (var workFlow in selectedWorkFlowsIds)
                {

                    var dbWorkFlow = await _zenithDbContext.WorkFlows.FirstOrDefaultAsync(x => x.Id == workFlow);
                    if (dbWorkFlow != null)
                        _zenithDbContext.WorkFlows.Remove(dbWorkFlow);
                    await _zenithDbContext.SaveChangesAsync();
                }
            }
            return true;
        }

        public async Task<WorkFlowsDTO> GetWorkFlowById(Guid workFlowId)
        {
            var workFlow = await _zenithDbContext.WorkFlows.
                Where(x => x.Id == workFlowId).
                Select(a => new WorkFlowsDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsActive = a.IsActive,
                    IsCritical = a.IsCritical,
                    Description = a.Description
                }).FirstOrDefaultAsync();

            return workFlow;
        }

        public async Task<bool> CreateWorkFlow(WorkFlowsDTO workFlowDto, string loggedInUserId)
        {
            try
            {
                if (workFlowDto == null)
                    throw new ArgumentNullException(nameof(workFlowDto));

                var newWorkFlow = new WorkFlows
                {
                    Id = Guid.NewGuid(),
                    Name = workFlowDto.Name,
                    IsActive = workFlowDto.IsActive,
                    IsCritical = workFlowDto.IsCritical,
                    Description = workFlowDto.Description,
                    CreatedBy = loggedInUserId,
                    CreatedOn = DateTime.UtcNow
                };

                _workFlowRepository.Add(newWorkFlow);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> UpdateWorkFlow(WorkFlowsDTO workFlowDto, string loggedInUserId)
        {
            if (workFlowDto == null)
                throw new ArgumentNullException(nameof(workFlowDto));

            var dbWorkFlow = await _zenithDbContext.WorkFlows.FirstOrDefaultAsync(x => x.Id == workFlowDto.Id);

            if (dbWorkFlow == null)
                return false;

            dbWorkFlow.Name = workFlowDto.Name;
            dbWorkFlow.IsActive = workFlowDto.IsActive;
            dbWorkFlow.IsCritical = workFlowDto.IsCritical;
            dbWorkFlow.Description = workFlowDto.Description;
            dbWorkFlow.ModifiedBy = loggedInUserId;
            dbWorkFlow.ModifiedOn = DateTime.UtcNow;

            _zenithDbContext.WorkFlows.Update(dbWorkFlow);
            var rowsAffected = await _zenithDbContext.SaveChangesAsync();

            return rowsAffected > 0;
        }

    }
}
