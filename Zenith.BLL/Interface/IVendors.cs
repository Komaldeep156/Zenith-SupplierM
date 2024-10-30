using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IVendors
    {
        List<GetVendorsListDTO> GetVendors();
        int AddVendor(VendorDTO model);
        Task<string>  UpdateVendor(updateVendorDTO model);
        GetVendorsListDTO GetVendorById(Guid vendorId);
        int AddAddress(AddressDTO model);
        string AddNewRegistration(RegistrationDTO model);
        string AddQualityCertification(QualityCertificationDTO model);
        string AddPaymentTerms(PaymentTermsDTO model);
        string AddAccountDetails(AccountDetailsDTO model);
        string AddOtherDocuments(OtherDocumentsDTO model);
        public List<GetVendorsListDTO> SearchVendorList(string fieldName, string searchText);
    }
}
