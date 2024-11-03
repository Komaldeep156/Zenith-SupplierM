using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.DomainModels
{
    public class Manufacturer: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string ManufacturerCode { get; set; }
        public string ShortName { get; set; }
        public string Website { get; set; }
        public string FullName { get; set; }
        public Guid RegisteredSinceId { get; set; }
        public Guid HeadQuarterId { get; set; }
        public bool IsActive { get; set; }
        public int RevisionNumer { get; set; }
        public string ApprovalStatus { get; set; }
        public string RejectionReason { get; set; }
        public Guid AddressId { get; set; }


        [ForeignKey("RegisteredSinceId")]
        public virtual DropdownValues DropdownValues_RegisteredSince { get; set; }
        [ForeignKey("HeadQuarterId")]
        public virtual DropdownValues DropdownValues_HeadQuarter { get; set; }

    }
}
