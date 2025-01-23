using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IVendorQualificationWorkFlowExecution
    {
        /// <summary>
        /// Retrieves a list of vendor qualification workflow executions.
        /// </summary>
        /// <param name="VendorQualificationWorkFlowExecutionToUserId">The ID of the user to whom the workflow execution is assigned.</param>
        /// <returns>A list of vendor qualification workflow execution DTOs.</returns>
        Task<List<VendorQualificationWorkFlowExecutionDTO>> GetVendorQualificationWorkFlowExecution(string VendorQualificationWorkFlowExecutionToUserId);

        /// <summary>
        /// Adds a new vendor qualification workflow execution.
        /// </summary>
        /// <param name="model">The vendor qualification workflow execution DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>The ID of the newly created vendor qualification workflow execution.</returns>
        Task<Guid> AddVendorQualificationWorkFlowExecution(VendorQualificationWorkFlowExecutionDTO model, string loggedInUserId);

        /// <summary>
        /// Retrieves a vendor qualification workflow execution by its ID.
        /// </summary>
        /// <param name="VendorQualificationWorkFlowExecutionId">The ID of the vendor qualification workflow execution.</param>
        /// <returns>A vendor qualification workflow execution DTO.</returns>
        Task<VendorQualificationWorkFlowExecutionDTO> GetVendorQualificationWorkFlowExecutionById(Guid VendorQualificationWorkFlowExecutionId);

        /// <summary>
        /// Updates an existing vendor qualification workflow execution.
        /// </summary>
        /// <param name="model">The vendor qualification workflow execution DTO.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> UpdateVendorQualificationWorkFlowExecution(VendorQualificationWorkFlowExecutionDTO model);

        /// <summary>
        /// Updates the status of a vendor qualification workflow execution from the workbench.
        /// </summary>
        /// <param name="model">The vendor qualification workflow execution DTO.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> UpdateVendorQualificationWorkFlowExecutionStatusFromWorkBench(VendorQualificationWorkFlowExecutionDTO model);

        /// <summary>
        /// Updates the statuses of multiple vendor qualification workflow executions.
        /// </summary>
        /// <param name="vendorIds">A list of vendor IDs.</param>
        /// <param name="status">The new status.</param>
        /// <param name="modifiedBy">The ID of the user who modified the records.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> UpdateVendorQualificationWorkFlowExecutionStatus(List<string> vendorIds, string status,string modifiedBy);
        
        /// <summary>
        /// Delegates requested vendors to a manager.
        /// </summary>
        /// <param name="delegationRequests">The delegation requests.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> DelegateRequestedAssignVendorsToManager(DelegationRequests delegationRequests, string loggedInUserId);


    }
}
