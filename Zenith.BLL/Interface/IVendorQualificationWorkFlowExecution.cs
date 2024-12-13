﻿using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IVendorQualificationWorkFlowExecution
    {
        Task<List<VendorQualificationWorkFlowExecutionDTO>> GetVendorQualificationWorkFlowExecution(string VendorQualificationWorkFlowExecutionToUserId);
        Task<Guid> AddVendorQualificationWorkFlowExecution(VendorQualificationWorkFlowExecutionDTO model, string loggedInUserId);
        Task<VendorQualificationWorkFlowExecutionDTO> GetVendorQualificationWorkFlowExecutionById(Guid VendorQualificationWorkFlowExecutionId);
        Task<bool> UpdateVendorQualificationWorkFlowExecution(VendorQualificationWorkFlowExecutionDTO model);
        Task<bool> DelegateRequestedAssignVendorsToManager(DelegationRequests delegationRequests, string loggedInUserId);
        Task<bool> UpdateVendorQualificationWorkFlowExecutionStatusFromWorkBench(VendorQualificationWorkFlowExecutionDTO model);
        Task<bool> UpdateVendorQualificationWorkFlowExecutionStatus(List<string> vendorIds, string status,string modifiedBy);

    }
}
