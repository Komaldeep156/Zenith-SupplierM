using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class VendorsInitializationForm: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RequestedBy { get; set; }
        public Guid PriorityId { get; set; }
        public DateTime RequiredBy { get; set; }
        public string SupplierName { get; set; }
        public Guid SupplierTypeId { get; set; }
        public string Scope { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public Guid ContactCountryId { get; set; }
        public string BusinessCard { get; set; }
        public string Website { get; set; }
        public string RequestNum { get; set; }
        public Guid StatusId { get; set; }
        public bool IsCritical { get; set; }
        public bool IsApproved { get; set; }
        public Guid RejectionReasonId { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }

        //[ForeignKey("RequestedBy")]
        //public virtual ApplicationUser ApplicationUser_RequestedBy { get; set; }
        [ForeignKey("PriorityId")]
        public virtual DropdownValues DropdownValues_Priority { get; set; } 
        
        [ForeignKey("RejectionReasonId")]
        public virtual DropdownValues DropdownValues_RejectionReason { get; set; }
        
        [ForeignKey("StatusId")]
        public virtual DropdownValues DropdownValues_Status { get; set; } 
        
        [ForeignKey("SupplierTypeId")]
        public virtual DropdownValues DropdownValues_SupplierType { get; set; }  
        
        [ForeignKey("ContactCountryId")]
        public virtual DropdownValues DropdownValues_ContactCountry { get; set; }

    }
}
