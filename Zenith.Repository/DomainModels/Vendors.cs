using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class Vendors: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string SupplierCode { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string Website { get; set; }
        public Guid SupplierCategoryId { get; set; }
        public Guid SupplierScopeId { get; set; }
        public Guid TenantId { get; set; }
        public bool IsCriticalApproved { get; set; }
        public bool IsActive { get; set; }
        public string AssignedRoleId { get; set; }
        public int RevisionNumber { get; set; }
        public string ApprovalStatus { get; set; }
        public string RejectionReason { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenants Tenant { get; set; }

        [ForeignKey("SupplierCategoryId")]
        public virtual DropdownValues DropdownValues_SupplierCategory { get; set; }
        [ForeignKey("SupplierScopeId")]
        public virtual DropdownValues DropdownValues_SupplierScope { get; set; }

    }
}
