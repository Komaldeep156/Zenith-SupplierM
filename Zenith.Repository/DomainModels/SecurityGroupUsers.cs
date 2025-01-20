using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class SecurityGroupUsers : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SecurityGroupId { get; set; }
        public string UserId { get; set; }

        [ForeignKey("SecurityGroupId")]
        public virtual SecurityGroups SecurityGroup_SecurityGroupId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

    }
}
