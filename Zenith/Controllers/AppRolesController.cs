using Microsoft.AspNetCore.Identity;
using Zenith.Repository.DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Zenith.Controllers
{
    [Authorize]
    public class AppRolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public AppRolesController(IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> UserManager)
        : base(httpContextAccessor, signInManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public List<IdentityRole> GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return roles;
        }
        public async Task<ActionResult> AddNewRole( IdentityRole model)
        {
            if (!await _roleManager.RoleExistsAsync(model.Name))
            {
                var newRole = new ApplicationRoles
                {
                    Name = model.Name,
                };

                await _roleManager.CreateAsync(newRole);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteById(string roleID)
        {
            try
            {
                var deleteRoleObj = _roleManager.Roles.Where(x => x.Id == roleID).FirstOrDefault();
                if (deleteRoleObj != null)
                {
                    await _roleManager.DeleteAsync(deleteRoleObj);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
