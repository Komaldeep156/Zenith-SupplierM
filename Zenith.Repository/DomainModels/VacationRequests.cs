using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class VacationRequests : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public int RequestNum { get; set; }
        public Guid StatusId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsApproved { get; set; }
        public Guid? RejectionReasonId { get; set; }
        public string? Comments { get; set; }
        public bool IsActive { get; set; }
        public string? RequestedByUserId { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser CreatedByUser { get; set; } 
        
        [ForeignKey("RequestedByUserId")]
        public virtual ApplicationUser RequestedByUser { get; set; }

        
        [ForeignKey("RejectionReasonId")]
        public virtual DropdownValues RejectionReason { get; set; }
        
        [ForeignKey("StatusId")]
        public virtual DropdownValues Status { get; set; } 
        
    }
}
