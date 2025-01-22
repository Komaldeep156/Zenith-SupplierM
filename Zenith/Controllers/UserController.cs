using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Transactions;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.BLL.Logic;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Zenith.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUser _IUser;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDropdownList _IDropdownList;
        private readonly IVacationRequests _iVacationRequests;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IVendorQualificationWorkFlow _vendorQualificationWorkFlow;
        private readonly ISecurityGroup _securityGroup;
        private readonly ISecurityGroupUsersLogic _securityGroupUsersLogic;

        public UserController(IUser IUser, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IDropdownList iDropdownList, RoleManager<IdentityRole> roleManager, IVacationRequests iVacationRequests, IVendorQualificationWorkFlow vendorQualificationWorkFlow, ISecurityGroup securityGroup, ISecurityGroupUsersLogic securityGroupUsersLogic) : base(httpContextAccessor, signInManager)
        {
            _userManager = userManager;
            _IUser = IUser;
            _IDropdownList = iDropdownList;
            _roleManager = roleManager;
            _iVacationRequests = iVacationRequests;
            _vendorQualificationWorkFlow = vendorQualificationWorkFlow;
            _securityGroup = securityGroup;
            _securityGroupUsersLogic = securityGroupUsersLogic;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
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

                data.AssignedSecurityGroups = await _securityGroup.GetSecurityGroupsAssignedToUser(userId);
                return View(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateUser(RegisterUserModel model)
        {
            try
            {
                if (!model.IsActive)
                {
                    var allReportingUserToThisUser = await _IUser.GetAllUsersReportingToThisUser(model.userId);
                    if (allReportingUserToThisUser.Any())
                    {
                        string commaSeparatedNames = string.Join(", ", allReportingUserToThisUser.Select(user => user.FullName));
                        return new JsonResult(new { ResponseCode = 1, Response = commaSeparatedNames });
                    }
                }

                var user = _userManager.Users.FirstOrDefault(x => x.Id == model.userId);

                if (user == null)
                {
                    return new JsonResult(new { ResponseCode = 2, Response = string.Empty });
                }

                var roleNames = await _userManager.GetRolesAsync(user);
                var roleIds = _roleManager.Roles
                                          .Where(role => roleNames.Contains(role.Name))
                                          .Select(role => role.Id)
                                          .ToList();

                if (!roleIds.Contains(model.RoleId))
                {
                    if (await _vendorQualificationWorkFlow.UserAnyWorkIsPending(model.userId))
                    {
                        return new JsonResult(new { ResponseCode = 3, Response = string.Empty });
                    }
                }

                await _securityGroupUsersLogic.RemoveAllSecurityGroupUsersAssignedToUserID(model.userId);
                var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (model.AssignedSecurityGroups != null)
                {
                    foreach (var securityGroupId in model.AssignedSecurityGroups)
                    {
                        var securityGroupUsers = new SecurityGroupUsersDTO
                        {
                            UserId = model.userId,
                            SecurityGroupId = securityGroupId,
                            CreatedBy = loginUserId,
                            CreatedOn = DateTime.Now,
                        };
                        await _securityGroupUsersLogic.AddSecurityGroupUsers(securityGroupUsers);
                    }
                }
                await _IUser.UpdateUser(model);

                if (!model.IsActive)
                {
                    await _iVacationRequests.CancelAllActiveVacationRequestsByUserId(model.userId);
                }

                return new JsonResult(new { ResponseCode = 2, Response = string.Empty });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet]
        public List<GetUserListDTO> GetUsers()
        {
            return _IUser.GetUsers();
        }


        [HttpGet]
        public JsonResult GetUsersJsonList()
        {
            var users = _IUser.GetUsers(); // Fetch users from the IUser service
            var activeUsers = users.Where(x=>x.IsActive).Select(x => new { x.Id,x.FullName }).ToList();
            return new JsonResult(activeUsers, new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            });
        }

        [HttpGet]
        public async Task<List<ApplicationUser>> GetReportingManagersAsync()
        {
            var reportingManagerList = await _IUser.GetReportingManagersAsync();
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
        public async Task<IActionResult> DeleteUsers([FromBody] List<string> selectedUserGuids)
        {
            try
            {
                List<KeyValuePair<string, string>> canNotDeleteReportingManager = new List<KeyValuePair<string, string>>();
                foreach (var userId in selectedUserGuids)
                {
                    var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                    if (user != null)
                    {
                        if ((await _IUser.GetAllUsersReportingToThisUser(userId)).Any())
                        {
                            canNotDeleteReportingManager.Add(new KeyValuePair<string, string>(user.FullName, "ReportingManager"));
                            continue;
                        }

                        //if ((! await _IUser.CanDeleteUserAsync(userId)) || (await _IUser.GetAllUsersReportingToThisUser(userId)).Any())
                        //{
                        //    canNotDeleteUsers.Add(user.FullName);
                        //    continue;
                        //}
                        using (var trasaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            try
                            {
                                var userRoles = await _userManager.GetRolesAsync(user);
                                // Remove user from all roles
                                if (userRoles.Any())
                                {
                                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                                }

                                // Delete the user
                                await _userManager.DeleteAsync(user);

                                trasaction.Complete();
                            }
                            catch (Exception ex)
                            {
                                canNotDeleteReportingManager.Add(new KeyValuePair<string, string>(user.FullName, "ReferenceProblem"));
                            }
                        }

                    }
                }
                // Return the list of users that could not be deleted as JSON
                return Ok(new
                {
                    CanNotDeleteUsers = canNotDeleteReportingManager
                });

            }
            catch (Exception ex)
            {
                // Return a structured error response
                return StatusCode(500, new
                {
                    ErrorMessage = "An error occurred while deleting users.",
                    ExceptionMessage = ex.Message
                });
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
