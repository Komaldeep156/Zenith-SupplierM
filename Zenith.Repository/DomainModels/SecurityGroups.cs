using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class SecurityGroups : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SecurityGroupCode { get; set; }
        public string Description {get;set; }
        public bool IsActive { get; set; }

    }
}
