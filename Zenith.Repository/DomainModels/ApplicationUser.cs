using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string UserCode { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ReportingManagerId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? CountryId { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("BranchId")]
        public virtual DropdownValues Branch { get; set; }

        [ForeignKey("CountryId")]
        public virtual DropdownValues Country { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual DropdownValues DropdownValues_Department { get; set; }
    }
}
