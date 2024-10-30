using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class Attachments:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Url { get; set; }
    }
}
