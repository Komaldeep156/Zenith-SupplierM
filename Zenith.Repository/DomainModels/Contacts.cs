using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.DomainModels
{
    public class Contacts: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid GenderId { get; set; }
        public int Phone { get; set; }
        public int Mobile { get; set; }
        public int Whatsapp { get; set; }
        public string Email { get; set; }
        public Guid PositionId { get; set; }
        public Guid ContactProfileId { get; set; }
        public Guid PrimaryContactId { get; set; }

        [ForeignKey("GenderId")]
        public virtual DropdownValues DropdownValues_Gender { get; set; }
        [ForeignKey("PositionId")]
        public virtual DropdownValues DropdownValues_Position { get; set; }
        [ForeignKey("ContactProfileId")]
        public virtual DropdownValues DropdownValues_ContactProfile { get; set; }
        [ForeignKey("PrimaryContactId")]
        public virtual DropdownValues DropdownValues_PrimaryContact { get; set; }

    }
}
