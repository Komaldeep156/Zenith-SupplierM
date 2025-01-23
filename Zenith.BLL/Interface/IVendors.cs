using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IVendors
    {
        /// <summary>
        /// Retrieves a list of vendors.
        /// </summary>
        /// <param name="assignUserId">The ID of the assigned user.</param>
        /// <returns>A vendor view model containing a list of vendors.</returns>
        Task<VendorViewModel> GetVendors(string assignUserId = default);

        /// <summary>
        /// Adds a new vendor.
        /// </summary>
        /// <param name="model">The vendor DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        Task<int> AddVendor(VendorDTO model, string loggedInUserId);

        /// <summary>
        /// Updates an existing vendor.
        /// </summary>
        /// <param name="model">The update vendor DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        Task<string> UpdateVendor(updateVendorDTO model, string loggedInUserId);

        /// <summary>
        /// Updates an existing vendor.
        /// </summary>
        /// <param name="model">The update vendor DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        Task<bool> UpdateVendorCriticalNonCritical(Guid vendorId, bool isVendorCritical);

        /// <summary>
        /// Retrieves a vendor by its ID.
        /// </summary>
        /// <param name="VendorsInitializationFormId">The ID of the vendor.</param>
        /// <returns>A vendor DTO.</returns>
        GetVendorsListDTO GetVendorById(Guid VendorsInitializationFormId);

        /// <summary>
        /// Deletes multiple vendors.
        /// </summary>
        /// <param name="selectedVendorIds">A list of vendor IDs to be deleted.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        bool DeleteVendors(List<Guid> selectedVendorIds);

        /// <summary>
        /// Adds a new address.
        /// </summary>
        /// <param name="model">The address DTO.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        int AddAddress(AddressDTO model);

        /// <summary>
        /// Adds a new registration.
        /// </summary>
        /// <param name="model">The registration DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        string AddNewRegistration(RegistrationDTO model);

        /// <summary>
        /// Adds a new quality certification.
        /// </summary>
        /// <param name="model">The quality certification DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        string AddQualityCertification(QualityCertificationDTO model);

        /// <summary>
        /// Adds new payment terms.
        /// </summary>
        /// <param name="model">The payment terms DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        string AddPaymentTerms(PaymentTermsDTO model);

        /// <summary>
        /// Adds new account details.
        /// </summary>
        /// <param name="model">The account details DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        string AddAccountDetails(AccountDetailsDTO model);

        /// <summary>
        /// Adds other documents.
        /// </summary>
        /// <param name="model">The other documents DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        string AddOtherDocuments(OtherDocumentsDTO model);

        /// <summary>
        /// Searches for vendors based on the provided filters.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="searchText">The search text to filter vendors.</param>
        /// <param name="assignUserId">The ID of the assigned user.</param>
        /// <returns>A vendor view model containing a list of vendors.</returns>
        Task<VendorViewModel> SearchVendorList(string fieldName, string searchText, string assignUserId = default);

        /// <summary>
        /// Updates the details of a vendor.
        /// </summary>
        /// <param name="model">The vendor DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> UpdateVendorDetails(VendorDTO model, string loggedInUserId);

        /// <summary>
        /// Checks if a vendor has a duplicate business registration number combination.
        /// </summary>
        /// <param name="model">The vendor initialization form model.</param>
        /// <returns>A boolean indicating whether a duplicate exists.</returns>
        Task<bool> IsDuplicateBusinesReqNoCombinetion(VendorsInitializationForm model);
    }
}
