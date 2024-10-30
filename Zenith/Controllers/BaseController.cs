using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    public class BaseController : Controller
    {

        protected string LoggedInUserRoleName = "";
        protected string LoggedInUserRoleID = "";
        protected string LoggedInUserName = "";
        protected int LoggedInUserID = 0;
        protected string LoggedInUserEmail = "";
        //protected int LoggedInUserCompanyID = 0;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpContext _HttpContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public BaseController(IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager) {
            _httpContextAccessor = httpContextAccessor;
            _HttpContext = httpContextAccessor.HttpContext;
           _signInManager = signInManager;
            if (_HttpContext != null)
            {
                //this.LoggedInUserCompanyID = Convert.ToInt32(_HttpContext.Session.GetString("LoggedInUserCompanyID"));
                this.LoggedInUserEmail = _HttpContext.Session.GetString("LoggedInUserEmail");

                if (LoggedInUserID == null)
                {
                    _ = KillIdentitySessions();
                }
            }
        }

        public async Task<IActionResult> KillIdentitySessions()
        {
            await _HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).ConfigureAwait(true);
            await _signInManager.SignOutAsync().ConfigureAwait(true);
            return RedirectToPage("/Identity/Account/Logout?returnUrl=%2F");
        }
    }
}
