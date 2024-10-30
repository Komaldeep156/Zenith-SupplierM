using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class DropdownValues : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Value { get; set; }
        public Guid DropdownParentNameId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
