using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

namespace Zenith.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUser _IUser;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDropdownList _IDropdownList;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(IUser IUser, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, IDropdownList iDropdownList, RoleManager<IdentityRole> roleManager) : base(httpContextAccessor, signInManager)
        {
            _userManager = userManager;
            _IUser = IUser;
            _IDropdownList = iDropdownList;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async  Task<IActionResult> Index()
        {
            var data = _IUser.GetUsers();
            ViewBag.ReportingManagerList = await _IUser.GetReportingManagersAsync();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> UserViewTemplate(string userId)
        {
            try
            {

                var rolesDDl = _roleManager.Roles.ToList();
                var countryDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.COUNTRY));
                var departmentDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.DEPARTMENTS));
                var branchDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.BRANCH));
                ViewBag.country = countryDDL;
                ViewBag.department = departmentDDL;
                ViewBag.branch = branchDDL;
                ViewBag.roles = rolesDDl;
                var data = await _IUser.GetUserByIdAsync(userId);
                List<ApplicationUser> reportingMangerDDL = await _IUser.GetReportingManagersAsync();
                ViewBag.reportingManager = reportingMangerDDL;
                return View(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public List<GetUserListDTO> GetUsers()
        {
            return _IUser.GetUsers();
        }
        
        [HttpGet]
        public async Task<List<ApplicationUser>> GetReportingManagersAsync()
        {
            var reportingManagerList= await _IUser.GetReportingManagersAsync();
            return reportingManagerList;
        }

        [HttpPost]
        public async Task<string> AddNewUser(RegisterUserModel model)
        {
            try
            {
                var requestScheme = Request.Scheme;
                return await _IUser.AddNewUser(model, Url, requestScheme);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<bool> DeleteUsers([FromBody] List<string> selectedUserGuids)
        {
            try
            {
                foreach (var userId in selectedUserGuids)
                {
                    var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                    if (user != null)
                    {
                        // Retrieve all roles assigned to the user
                        var userRoles = await _userManager.GetRolesAsync(user);

                        // Remove user from all roles
                        if (userRoles.Any())
                        {
                            await _userManager.RemoveFromRolesAsync(user, userRoles);
                        }

                        // Delete the user
                        await _userManager.DeleteAsync(user);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> UpdateUser(RegisterUserModel model)
        {
            try
            {
                return await _IUser.UpdateUser(model);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<bool> UpdateUserActiveInactive(string userId, bool isActive)
        {
            try
            {
                return await _IUser.UpdateUserActiveInactive(userId, isActive);
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> DeleteById(string userId)
        {
            try
            {
                var deleteObj = _userManager.Users.Where(x => x.Id == userId).FirstOrDefault();
                if (deleteObj != null)
                {
                    await _userManager.DeleteAsync(deleteObj);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult AddContact(ContactDTO model)
        {
            return Json(_IUser.AddContact(model));
        }
        public JsonResult AddFile(AttachmentDTO File)
        {
            return Json(_IUser.AddFile(File));
        }
    }
}
