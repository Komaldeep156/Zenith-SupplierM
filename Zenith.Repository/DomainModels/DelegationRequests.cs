using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.DomainModels
{
    public class DelegationRequests : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string ApprovalType { get; set; }
        public Guid SourceId { get; set; }
        public string DelegateFromUserId { get; set; }
        public Guid StatusId { get; set; }
        public string DelegateToUserId { get; set; }


        [ForeignKey("StatusId")]
        public virtual DropdownValues Status { get; set; }

        [ForeignKey("DelegateFromUserId")]
        public virtual ApplicationUser DelegateFromUser { get; set; }

        [ForeignKey("DelegateToUserId")]
        public virtual ApplicationUser DelegateToUser { get; set; }

    }
}