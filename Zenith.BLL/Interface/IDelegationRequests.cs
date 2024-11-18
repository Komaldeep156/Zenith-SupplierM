using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IDelegationRequests
    {
        public Task<bool> AddNew(CreateDelegateRequestDTO model, string loggedInUserId);
        Task<List<GetDelegateRequestDTO>> GetDelegationRequests(string delegateToUserId);
        Task<bool> AcceptOrRejectDelegateRequest(Guid delegateRequestId, bool isDelegationReqAccepted);
    }
}
