using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IWorkFlows
    {
        /// <summary>
        /// Retrieves a list of workflows.
        /// </summary>
        /// <returns>A list of workflow DTOs.</returns>
        Task<List<WorkFlowsDTO>> GetWorkFlows();

        /// <summary>
        /// Deletes multiple workflows.
        /// </summary>
        /// <param name="selectedWorkFlowsIds">A list of workflow IDs to be deleted.</param>
        /// <returns>A tuple containing a boolean indicating success and a list of workflow names that were not deleted.</returns>
        Task<(bool isSuccess, List<string> notDeletedWorkFlowNames)> DeleteWorkFlows(List<Guid> selectedWorkFlowsIds);

        /// <summary>
        /// Retrieves a workflow by its ID.
        /// </summary>
        /// <param name="workFlowId">The ID of the workflow.</param>
        /// <returns>A workflow DTO.</returns>
        Task<WorkFlowsDTO> GetWorkFlowById(Guid workFlowId);

        /// <summary>
        /// Creates a new workflow.
        /// </summary>
        /// <param name="workFlowDto">The workflow DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> CreateWorkFlow(WorkFlowsDTO workFlowDto, string loggedInUserId);

        /// <summary>
        /// Updates an existing workflow.
        /// </summary>
        /// <param name="workFlowDto">The workflow DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> UpdateWorkFlow(WorkFlowsDTO workFlowDto, string loggedInUserId);
    }
}
