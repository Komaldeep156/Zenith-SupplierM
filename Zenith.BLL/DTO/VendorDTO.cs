using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class VendorDTO:VendorsInitializationForm
    {

    }

    public class updateVendorDTO {
        public Guid VendorsInitializationFormId { get; set; }
        public string Website { get; set; }
        public Guid RejectionReasonId { get; set; }
        public string AssignedRoleId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsCriticalApproved { get; set; }
        public bool IsReject {  get; set; }
        public string comments { get; set; }        
    }

    public class GetVendorsListDTO :VendorDTO
    {
        public  DropdownValues SupplierCategory {get; set;}
        public  DropdownValues SupplierScope { get; set; }
    }

    public class RegistrationDTO
    {
        public Guid UserId { get; set; }
        public Guid LicenseTypeId { get; set; }
        public int LicenseNumber { get; set; }
        public int RegisteredSince { get; set; }
        public Guid RegistrationCategoryId { get; set; }
        public Guid RegisteredCountryId { get; set; }
        public Guid RegistrationValidityId { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
        public IFormFile Document { get; set; }
        public int ActivityCode { get; set; }
        public Guid ActivityName { get; set; }
    }

    public class QualityCertificationDTO
    {
        public Guid UserId { get; set; }
        public Guid LicenseById { get; set; }
        public Guid LicenseStandardId { get; set; }
        public int LicenseNumber { get; set; }
        public Guid RegistrationValidityId { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
        public IFormFile Document { get; set; }
        public string LicenseScope { get; set; }
    }

    public class PaymentTermsDTO
    {
        public Guid UserId { get; set; }
        public int CreditTerms { get; set; }
        public int CreditLimit { get; set; }
        public int CreditLimitCurrency { get; set; }
        public IFormFile Ducument { get; set; }
    }

    public class AccountDetailsDTO
    {
        public Guid UserId { get; set; }
        public int BeneficiaryName { get; set; }
        public Guid BankNameId { get; set; }
        public Guid BranchNameId { get; set; }
        public Guid BankCountryId { get; set; }
        public int AccountNumber { get; set; }
        public Guid AccountCurrencyId { get; set; }
        public Guid IBANId { get; set; }
        public Guid SwiftCodeId { get; set; }
        public Guid IfscCodeId { get; set; }
        public Guid BankAddressId { get; set; }
        public IFormFile BankLetter { get; set; }
    }

    public class OtherDocumentsDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DocumentName { get; set; }
        public Guid DocumentTypeId { get; set; }
        public IFormFile Document { get; set; }
        public Guid ExpiryDateId { get; set; }
    }
}
