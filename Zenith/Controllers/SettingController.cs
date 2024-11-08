using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    public class SettingController : BaseController
    {
        private readonly ISetting _ISetting;
        private readonly IUser _IUser;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        public SettingController(IHttpContextAccessor httpContextAccessor, ISetting ISetting, IUser Iuser,
            SignInManager<ApplicationUser> signInManager) : base(httpContextAccessor, signInManager)
        {
            _ISetting = ISetting;
            _SignInManager = signInManager;
            _IUser = Iuser;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = _IUser.GetUserById(loggedInUserId);
            return View(data);
        }

        public async Task<string> ChangePassword(string currentPassword, string newPassword)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _ISetting.ChangePassword(currentPassword, newPassword, loggedInUserId);
        }
    }
}
