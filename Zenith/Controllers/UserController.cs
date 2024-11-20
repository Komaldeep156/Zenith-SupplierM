using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
        private readonly IVacationRequests _iVacationRequests;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUser IUser, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, IDropdownList iDropdownList, RoleManager<IdentityRole> roleManager, IVacationRequests iVacationRequests) : base(httpContextAccessor, signInManager)
        {
            _userManager = userManager;
            _IUser = IUser;
            _IDropdownList = iDropdownList;
            _roleManager = roleManager;
            _iVacationRequests = iVacationRequests;
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
        public async Task<string> DeleteUsers([FromBody] List<string> selectedUserGuids)
        {
            try
            {
                List<string> canNotDeleteUsers = new List<string>();
                foreach (var userId in selectedUserGuids)
                {
                    var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                    if (user!=null)
                    {
                        if ((! await _IUser.CanDeleteUserAsync(userId)) || (await _IUser.GetAllUsersReportingToThisUser(userId)).Any())
                        {
                            canNotDeleteUsers.Add(user.FullName);
                            continue;
                        }

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
                return string.Join(',',canNotDeleteUsers);
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }

        [HttpPost]
        public async Task<int> UpdateUser(RegisterUserModel model)
        {
            try
            {
                if (!model.IsActive)
                {
                    var allReportingUserToThisUser = await _IUser.GetAllUsersReportingToThisUser(model.userId);
                    if (allReportingUserToThisUser.Any())
                    {
                        return 1;
                    }
                }
                await _IUser.UpdateUser(model);

                if (!model.IsActive)
                {
                    await _iVacationRequests.CancelAllActiveVacationRequestsByUserId(model.userId);
                }

                return 2;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<int> UpdateUserActiveInactive(string userId, bool isActive)
        {
            try
            {
                if (!isActive)
                {
                    var allReportingUserToThisUser = await _IUser.GetAllUsersReportingToThisUser(userId);
                    if (allReportingUserToThisUser.Any())
                    {
                        return 1;
                    }
                }

                await _IUser.UpdateUserActiveInactive(userId, isActive);
                if (!isActive)
                {
                    await _iVacationRequests.CancelAllActiveVacationRequestsByUserId(userId);
                }

                return 2;

            }
            catch (Exception)
            {
                return -1;
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
