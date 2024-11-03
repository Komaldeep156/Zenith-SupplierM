using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class SecurityGroupsRoles : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SecurityGroupId { get; set; }
        public string RoleId { get; set; }


        [ForeignKey("SecurityGroupId")]
        public virtual SecurityGroups SecurityGroup { get; set; }
        
        [ForeignKey("RoleId")]
        public virtual ApplicationRoles ApplicationRoles { get; set; }
    }
}
