using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IVendors
    {
        List<GetVendorsListDTO> GetVendors();
        int AddVendor(VendorDTO model, string loggedInUserId);
        Task<string>  UpdateVendor(updateVendorDTO model);
        Task<bool> UpdateVendorCriticalNonCritical(Guid vendorId, bool isVendorCritical);
        GetVendorsListDTO GetVendorById(Guid VendorsInitializationFormId);
        int AddAddress(AddressDTO model);
        string AddNewRegistration(RegistrationDTO model);
        string AddQualityCertification(QualityCertificationDTO model);
        string AddPaymentTerms(PaymentTermsDTO model);
        string AddAccountDetails(AccountDetailsDTO model);
        string AddOtherDocuments(OtherDocumentsDTO model);
        public List<GetVendorsListDTO> SearchVendorList(string fieldName, string searchText);

    }
}
