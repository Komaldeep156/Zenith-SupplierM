using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; }
        public string UserCode { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? DepartmentId { get; set; }

        [ForeignKey("PositionId")]
        public virtual DropdownValues DropdownValues_Position { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DropdownValues DropdownValues_Department { get; set; }
    }
}
