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
        public Guid? PositionId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string userId { get; set; }
        public string Role { get; set; }
    }

    public class UserWithRoles : ApplicationUser
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
