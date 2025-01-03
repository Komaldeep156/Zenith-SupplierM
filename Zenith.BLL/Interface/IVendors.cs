using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IVendors
    {
        Task<List<GetVendorsListDTO>> GetVendors(string assignUserId = default);
        Task<int> AddVendor(VendorDTO model, string loggedInUserId);
        Task<string>  UpdateVendor(updateVendorDTO model, string loggedInUserId);
        Task<bool> UpdateVendorCriticalNonCritical(Guid vendorId, bool isVendorCritical);
        GetVendorsListDTO GetVendorById(Guid VendorsInitializationFormId);
        int AddAddress(AddressDTO model);
        string AddNewRegistration(RegistrationDTO model);
        string AddQualityCertification(QualityCertificationDTO model);
        string AddPaymentTerms(PaymentTermsDTO model);
        string AddAccountDetails(AccountDetailsDTO model);
        string AddOtherDocuments(OtherDocumentsDTO model);
        Task<List<GetVendorsListDTO>> SearchVendorList(string fieldName, string searchText, string assignUserId = default);
        bool DeleteVendors(List<Guid> selectedVendorIds);
        //Task<bool> UpdateVendorStatuses(List<string> vendorIds, string status);
        Task<bool> UpdateVendorDetails(VendorDTO model, string loggedInUserId);
        Task<bool> DuplicateBusinesReqNoCombinetion(VendorsInitializationForm model);
    }
}
