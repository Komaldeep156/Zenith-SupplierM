using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class OtherDocuments:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DocumentName { get; set; }
        public Guid DocumentTypeId { get; set; }
        public string Document { get; set; }
        public Guid ExpiryDateId { get; set; }

        [ForeignKey("DocumentTypeId")]
        public virtual DropdownValues DropdownValues_DocumentType { get; set; }
        [ForeignKey("ExpiryDateId")]
        public virtual DropdownValues DropdownValues_ExpiryDate { get; set; }
    }
}
