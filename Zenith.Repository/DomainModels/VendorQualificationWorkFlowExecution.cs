using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Zenith.Repository.DomainModels
{
    public class VendorQualificationWorkFlowExecution : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid VendorQualificationWorkFlowId { get; set; }
        public string AssignedUserId { get; set; }
        public bool IsActive { get; set; }
        public Guid VendorId { get; set; }
        public Guid StatusId { get; set; }

        [ForeignKey("StatusId")]
        public virtual DropdownValues DropdownValues_Status { get; set; }

        [ForeignKey("AssignedUserId")]
        public virtual ApplicationUser ApplicationUser_AssignedUser { get; set; }  
        
        [ForeignKey("VendorQualificationWorkFlowId")]
        public virtual VendorQualificationWorkFlow VendorQualificationWorkFlow { get; set; }
    }
}
