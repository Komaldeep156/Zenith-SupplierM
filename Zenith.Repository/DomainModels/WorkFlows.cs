using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class WorkFlows : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsCritical { get; set; }
        public string Description { get; set; }
    }
}
