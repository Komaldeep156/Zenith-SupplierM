﻿using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Retrieves a list of workflows.
        /// </summary>
        /// <returns>A list of workflow DTOs.</returns>
        public async Task<List<WorkFlowsDTO>> GetWorkFlows()
        {
            var workFlowList = await (
                from wf in _zenithDbContext.WorkFlows
                join usr in _zenithDbContext.Users
                    on wf.CreatedBy equals usr.Id into userJoin
                from user in userJoin.DefaultIfEmpty()
                select new WorkFlowsDTO
                {
                    Id = wf.Id,
                    Name = wf.Name,
                    IsActive = wf.IsActive,
                    IsCritical = wf.IsCritical,
                    Description = wf.Description,
                    CreatedOn = wf.CreatedOn,
                    CreatedBy = user != null ? user.FullName : ""
                })
                .ToListAsync();

            return workFlowList;
        }

        /// <summary>
        /// Deletes multiple workflows.
        /// </summary>
        /// <param name="selectedWorkFlowsIds">A list of workflow IDs to be deleted.</param>
        /// <returns>A tuple containing a boolean indicating success and a list of workflow names that were not deleted.</returns>
        public async Task<(bool isSuccess, List<string> notDeletedWorkFlowNames)> DeleteWorkFlows(List<Guid> selectedWorkFlowsIds)
        {
            List<string> notDeletedWorkFlowNames = new List<string>();
            bool isSuccess = true;

            if (selectedWorkFlowsIds != null)
            {
                foreach (var workFlowId in selectedWorkFlowsIds)
                {
                    var dbWorkFlow = await _zenithDbContext.WorkFlows.FirstOrDefaultAsync(x => x.Id == workFlowId);
                    if (dbWorkFlow != null)
                    {
                        try
                        {
                            _zenithDbContext.WorkFlows.Remove(dbWorkFlow);
                            await _zenithDbContext.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            _zenithDbContext.Entry(dbWorkFlow).State = EntityState.Unchanged;
                            notDeletedWorkFlowNames.Add(dbWorkFlow.Name);
                        }
                    }
                }

                if(selectedWorkFlowsIds.Count() == notDeletedWorkFlowNames.Count())
                   isSuccess = false;
            }

            return (isSuccess, notDeletedWorkFlowNames);
        }

        /// <summary>
        /// Retrieves a workflow by its ID.
        /// </summary>
        /// <param name="workFlowId">The ID of the workflow.</param>
        /// <returns>A workflow DTO.</returns>
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
                    Description = a.Description,
                    CreatedOn = a.CreatedOn,
                    CreatedBy = a.CreatedBy
                }).FirstOrDefaultAsync();

            return workFlow;
        }

        /// <summary>
        /// Creates a new workflow.
        /// </summary>
        /// <param name="workFlowDto">The workflow DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
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

        /// <summary>
        /// Updates an existing workflow.
        /// </summary>
        /// <param name="workFlowDto">The workflow DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
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
