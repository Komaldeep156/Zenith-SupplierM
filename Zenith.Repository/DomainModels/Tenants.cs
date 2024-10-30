using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class Tenants : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TenantCode { get; set; }
        public string? Logo { get; set; }
        public bool IsActive { get; set; }
        public string? Timezone { get; set; } // Central Standard Time Mountain Standard Time
        public string Website { get; set; }
       
    }
}
