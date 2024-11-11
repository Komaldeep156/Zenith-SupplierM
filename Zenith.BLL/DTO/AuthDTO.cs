using System.ComponentModel.DataAnnotations;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class AuthDTO
    {
    }

    public class LogInViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class RegisterUserModel
    {
        [Required]
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid BranchId { get; set; }
        public string ReportingManagerId { get; set; }
        public Guid CountryId { get; set; }
        public string RoleId { get; set; }
        public string FullName { get; set; }
        public string userId { get; set; }
        public bool IsActive { get; set; }
        public bool IsVacationModeOn { get; set; }
    }

    public class UserWithRoles : ApplicationUser
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
