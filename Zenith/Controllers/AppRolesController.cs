using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    [Authorize]
    public class AppRolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public AppRolesController(IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> UserManager)
        : base(httpContextAccessor, signInManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// This method retrieves a list of all roles in the system and returns the view to display them.
        /// </summary>
        /// <returns>Returns the view displaying the list of roles.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        /// <summary>
        /// This method retrieves and returns a list of all roles in the system.
        /// </summary>
        /// <returns>Returns a list of all roles in the system as a list of <see cref="IdentityRole"/> objects.</returns>
        [HttpGet]
        public List<IdentityRole> GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return roles;
        }

        /// <summary>
        /// This method adds a new role to the system if the role does not already exist.
        /// It checks for the role's existence, and if not found, creates a new role and redirects to the index page.
        /// </summary>
        /// <param name="model">The role data transfer object containing the details of the role to be added.</param>
        /// <returns>Redirects to the index page after attempting to add the new role.</returns>
        public async Task<ActionResult> AddNewRole(IdentityRole model)
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

        /// <summary>
        /// This method deletes a role by its unique ID. It checks if the role exists, deletes it if found, and redirects to the index page.
        /// If an error occurs, it throws the exception.
        /// </summary>
        /// <param name="roleID">The unique identifier of the role to be deleted.</param>
        /// <returns>Redirects to the index page after attempting to delete the specified role.</returns>
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
