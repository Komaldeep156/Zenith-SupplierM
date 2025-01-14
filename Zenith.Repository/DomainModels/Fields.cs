using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class Fields : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string WindowName { get; set; }
        public string SectionName { get; set; }
        public string TabName { get; set; }
        public string FieldCode { get; set; }
        public string FieldName { get; set; }
    }
}
