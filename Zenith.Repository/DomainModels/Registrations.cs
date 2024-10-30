using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class Registrations: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid LicenseTypeId { get; set; }
        public int LicenseNumber{ get; set; }
        public int RegisteredSince { get; set; }
        public Guid RegistrationCategoryId { get; set; }
        public Guid RegisteredCountryId { get; set; }
        public Guid RegistrationValidityId { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
        public string Document { get; set; }
        public int ActivityCode { get; set; }
        public Guid ActivityNameId { get; set; }

        public Guid VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual Vendors Vendor { get; set; }

        [ForeignKey("LicenseTypeId")]
        public virtual DropdownValues DropdownValues_LicenseType { get; set; }
        [ForeignKey("RegistrationCategoryId")]
        public virtual DropdownValues DropdownValues_RegistrationCategory { get; set; }
       
        [ForeignKey("RegisteredCountryId")]
        public virtual DropdownValues DropdownValues_RegisteredCountry { get; set; }

        [ForeignKey("RegistrationValidityId")]
        public virtual DropdownValues DropdownValues_RegistrationValidity { get; set; }
        [ForeignKey("ActivityNameId")]
        public virtual DropdownValues DropdownValues_ActivityName { get; set; }
    }
}
