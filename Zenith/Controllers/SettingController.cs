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
        private readonly SignInManager<ApplicationUser> _SignInManager;
        public SettingController(IHttpContextAccessor httpContextAccessor, ISetting ISetting,
            SignInManager<ApplicationUser> signInManager) : base(httpContextAccessor, signInManager)
        {
            _ISetting = ISetting;
            _SignInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<string> ChangePassword(string currentPassword, string newPassword)
        {
            var loginUser = _SignInManager.IsSignedIn(User);
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _ISetting.ChangePassword(currentPassword, newPassword, loggedInUserId);
        }
    }
}
