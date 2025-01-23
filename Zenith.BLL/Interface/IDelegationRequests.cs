using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IDelegationRequests
    {
        /// <summary>
        /// Adds a new delegation request.
        /// </summary>
        /// <param name="model">The model containing the details of the delegation request.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public Task<bool> AddNew(CreateDelegateRequestDTO model, string loggedInUserId);
        
        /// <summary>
        /// Retrieves a list of delegation requests for a specific user.
        /// </summary>
        /// <param name="delegateToUserId">The ID of the user to whom the requests are delegated.</param>
        /// <returns>A list of <see cref="GetDelegateRequestDTO"/> objects.</returns>
        Task<List<GetDelegateRequestDTO>> GetDelegationRequests(string delegateToUserId);
        
        /// <summary>
        /// Accepts or rejects a delegation request.
        /// </summary>
        /// <param name="delegateRequestId">The ID of the delegation request.</param>
        /// <param name="isDelegationReqAccepted">A boolean indicating whether the request is accepted.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> AcceptOrRejectDelegateRequest(Guid delegateRequestId, bool isDelegationReqAccepted, string loggedInUserId);
    }
}
