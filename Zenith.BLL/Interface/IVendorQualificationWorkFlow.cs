using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IVendorQualificationWorkFlow
    {
        /// <summary>
        /// Retrieves a list of vendor qualification workflows.
        /// </summary>
        /// <param name="workFlowId">The ID of the workflow.</param>
        /// <returns>A list of vendor qualification workflow DTOs.</returns>
        Task<List<VendorQualificationWorkFlowDTO>> GetVendorQualificationWorkFlow(Guid workFlowId = default);

        /// <summary>
        /// Adds a new vendor qualification workflow.
        /// </summary>
        /// <param name="model">The vendor qualification workflow DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>The ID of the newly created vendor qualification workflow.</returns>
        Task<Guid> AddVendorQualificationWorkFlow(VendorQualificationWorkFlowDTO model, string loggedInUserId);

        /// <summary>
        /// Retrieves a vendor qualification workflow by its ID.
        /// </summary>
        /// <param name="vendorQualificationWorkFlowId">The ID of the vendor qualification workflow.</param>
        /// <returns>A vendor qualification workflow DTO.</returns>
        Task<VendorQualificationWorkFlowDTO> GetVendorQualificationWorkFlowById(Guid vendorQualificationWorkFlowId);

        /// <summary>
        /// Updates an existing vendor qualification workflow.
        /// </summary>
        /// <param name="model">The vendor qualification workflow DTO.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> UpdateVendorQualificationWorkFlow(VendorQualificationWorkFlowDTO model);

        /// <summary>
        /// Deletes multiple vendor qualification workflows.
        /// </summary>
        /// <param name="vendorQualificationWorkFlowId">A list of vendor qualification workflow IDs.</param>
        /// <returns>A tuple containing a boolean indicating success and a list of workflow names that were not deleted.</returns>
        Task<(bool isSuccess, List<string> notDeletedVQWorkFlowNames)> DeleteVendorQualificationWorkFlow(List<Guid> vendorQualificationWorkFlowId);

        /// <summary>
        /// Checks if any work is pending for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A boolean indicating whether any work is pending for the user.</returns>
        Task<bool> UserAnyWorkIsPending(string userId);
    }
}
