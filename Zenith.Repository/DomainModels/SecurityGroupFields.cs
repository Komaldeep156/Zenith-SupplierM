using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class SecurityGroupFields : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SecurityGroupId { get; set; }
        public Guid FieldId { get; set; }

        [ForeignKey("SecurityGroupId")]
        public virtual SecurityGroups SecurityGroup_SecurityGroupId { get; set; }

        [ForeignKey("FieldId")]
        public virtual Fields Fields_FieldId { get; set; }

    }
}
