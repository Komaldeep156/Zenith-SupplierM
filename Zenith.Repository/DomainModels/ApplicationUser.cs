using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; }
        public Guid TenantId { get; set; }


        [ForeignKey("TenantId")]
        public virtual Tenants Tenant { get; set; }
    }
}
