using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IWorkFlows
    {
        Task<List<WorkFlowsDTO>> GetWorkFlows();
        Task<bool> DeleteWorkFlows(List<Guid> selectedWorkFlowsIds);
        Task<WorkFlowsDTO> GetWorkFlowById(Guid workFlowId);
        Task<bool> CreateWorkFlow(WorkFlowsDTO workFlowDto, string loggedInUserId);
        Task<bool> UpdateWorkFlow(WorkFlowsDTO workFlowDto, string loggedInUserId);
    }
}
