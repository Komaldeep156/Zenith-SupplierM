using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class QualityCertification: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid LicenseById { get; set; }
        public Guid LicenseStandardId { get; set; }
        public int LicenseNumber { get; set; }
        public Guid RegistrationValidityId { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
        public string Document { get; set; }
        public string LicenseScope { get; set; }
        public Guid VendorsInitializationFormId { get; set; }

        [ForeignKey("VendorsInitializationFormId")]
        public virtual VendorsInitializationForm Vendor { get; set; }

        [ForeignKey("LicenseById")]
        public virtual DropdownValues DropdownValues_LicenseBy { get; set; }
        [ForeignKey("LicenseStandardId")]
        public virtual DropdownValues DropdownValues_LicenseStandard { get; set; }
        [ForeignKey("RegistrationValidityId")]
        public virtual DropdownValues DropdownValues_RegistrationValidity { get; set; }
    }
}
