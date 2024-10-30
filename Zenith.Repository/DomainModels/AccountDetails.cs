using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class AccountDetails: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BeneficiaryNameId { get; set; }
        public Guid BankNameId { get; set; }
        public Guid BranchNameId { get; set; }
        public Guid BankCountryId { get; set; }
        public int AccountNumber { get; set; }
        public Guid AccountCurrencyId { get; set; }
        public Guid IBANId { get; set; }
        public Guid SwiftCodeId { get; set; }
        public Guid IfscCodeId { get; set; }
        public Guid BankAddressId { get; set; }
        public string BankLetter { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("BeneficiaryNameId")]
        public virtual DropdownValues DropdownValues_BeneficiaryName { get; set; }
        [ForeignKey("BankNameId")]
        public virtual DropdownValues DropdownValues_BankName { get; set; }
        [ForeignKey("BranchNameId")]
        public virtual DropdownValues DropdownValues_BranchName { get; set; }
        [ForeignKey("BankCountryId")]
        public virtual DropdownValues DropdownValues_BankCountry { get; set; }
        [ForeignKey("AccountCurrencyId")]
        public virtual DropdownValues DropdownValues_AccountCurrency { get; set; }
        [ForeignKey("IBANId")]
        public virtual DropdownValues DropdownValues_Iban { get; set; }
        [ForeignKey("SwiftCodeId")]
        public virtual DropdownValues DropdownValues_SwiftCode { get; set; }
        [ForeignKey("IfscCodeId")]
        public virtual DropdownValues DropdownValues_IfscCode { get; set; }
        [ForeignKey("BankAddressId")]
        public virtual DropdownValues DropdownValues_BankAddress { get; set; }
    }
}
