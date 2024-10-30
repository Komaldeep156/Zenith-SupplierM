using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class DropdownLists:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid TenantId { get; set; }


        [ForeignKey("TenantId")]
        public virtual Tenants Tenant { get; set; }

    }
}
