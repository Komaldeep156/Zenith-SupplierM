using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IVendorQualificationWorkFlow
    {
        Task<List<VendorQualificationWorkFlowDTO>> GetVendorQualificationWorkFlow(Guid workFlowId = default);
        Task<Guid> AddVendorQualificationWorkFlow(VendorQualificationWorkFlowDTO model, string loggedInUserId);
        Task<VendorQualificationWorkFlowDTO> GetVendorQualificationWorkFlowById(Guid vendorQualificationWorkFlowId);
        Task<bool> UpdateVendorQualificationWorkFlow(VendorQualificationWorkFlowDTO model);
        Task<bool> DeleteVendorQualificationWorkFlow(Guid vendorQualificationWorkFlowId);

    }
}
