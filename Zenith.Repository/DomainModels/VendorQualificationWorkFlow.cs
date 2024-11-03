using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class VendorQualificationWorkFlow : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SecurityGroupId { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsCriticalOnly { get; set; }

        [ForeignKey("SecurityGroupId")]
        public virtual SecurityGroups SecurityGroup { get; set; }

    }
}
