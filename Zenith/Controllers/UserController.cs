using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Transactions;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

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

        public UserController(IUser IUser, IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IDropdownList iDropdownList,
            RoleManager<IdentityRole> roleManager,
            IVacationRequests iVacationRequests,
            IVendorQualificationWorkFlow vendorQualificationWorkFlow,
            ISecurityGroup securityGroup,
            ISecurityGroupUsersLogic securityGroupUsersLogic) : base(httpContextAccessor, signInManager)
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

        /// <summary>
        /// Retrieves a list of users and reporting managers, and passes them to the view.
        /// </summary>
        /// <returns>A view displaying the list of users and reporting managers.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = _IUser.GetUsers();
            ViewBag.ReportingManagerList = await _IUser.GetReportingManagersAsync();
            return View(data);
        }

        /// <summary>
        /// Retrieves and prepares data for displaying user details, including roles, countries, departments, branches, reporting managers, and assigned security groups.
        /// </summary>
        /// <param name="userId">The ID of the user whose details are being retrieved.</param>
        /// <returns>A view displaying the user's details with dropdown lists for roles, countries, departments, branches, and reporting managers.</returns>
        [HttpGet]
        public async Task<IActionResult> UserViewTemplate(string userId)
        {
            try
            {
                var data = await _IUser.GetUserByIdAsync(userId);

                if (data == null)
                {
                    return NotFound("User data not found.");
                }

                data.CountryList = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.COUNTRY));
                data.DepartmentList = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.DEPARTMENTS));
                data.BranchList = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.BRANCH));
                data.RolesList = _roleManager.Roles.ToList();
                data.ReportingMangerList = await _IUser.GetReportingManagersAsync();
                data.AssignedSecurityGroups = await _securityGroup.GetSecurityGroupsAssignedToUser(userId);
                return View(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates user information including roles, assigned security groups, and vacation requests. Handles checks for active status and pending work.
        /// </summary>
        /// <param name="model">The model containing updated user information.</param>
        /// <returns>A JSON response indicating the result of the update operation.</returns>
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

        /// <summary>
        /// Retrieves a list of users.
        /// </summary>
        /// <returns>A list of users as GetUserListDTO objects.</returns>
        [HttpGet]
        public List<GetUserListDTO> GetUsers()
        {
            return _IUser.GetUsers();
        }

        /// <summary>
        /// Retrieves a list of active users and returns their Id and FullName in a structured JSON format.
        /// </summary>
        /// <returns>
        /// A JsonResult containing a list of active users with their Id and FullName.
        /// </returns>
        [HttpGet]
        public JsonResult GetUsersJsonList()
        {
            var users = _IUser.GetUsers(); // Fetch users from the IUser service
            var activeUsers = users.Where(x => x.IsActive).Select(x => new { x.Id, x.FullName }).ToList();
            return new JsonResult(activeUsers, new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            });
        }

        /// <summary>
        /// Asynchronously retrieves a list of reporting managers from the IUser service.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ApplicationUser"/> objects representing the reporting managers.
        /// </returns>
        [HttpGet]
        public async Task<List<ApplicationUser>> GetReportingManagersAsync()
        {
            var reportingManagerList = await _IUser.GetReportingManagersAsync();
            return reportingManagerList;
        }

        /// <summary>
        /// Asynchronously adds a new user using the provided <see cref="RegisterUserModel"/>.
        /// </summary>
        /// <param name="model">The user details to be added.</param>
        /// <returns>
        /// A string response indicating the result of the operation.
        /// </returns>
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

        /// <summary>
        /// Deletes the selected users, ensuring that no users who are reporting managers are deleted.
        /// If a user is a reporting manager or has references, the deletion will not be performed for that user.
        /// </summary>
        /// <param name="selectedUserGuids">A list of GUIDs representing the users to be deleted.</param>
        /// <returns>
        /// A JSON response containing a list of users who could not be deleted and the reason.
        /// </returns>
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

                        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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

                                transaction.Complete();
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

        /// <summary>
        /// Updates the active/inactive status of a user. If the user is being deactivated and has reporting users, the deactivation is prevented.
        /// If the user is deactivated, all active vacation requests are also canceled.
        /// </summary>
        /// <param name="userId">The ID of the user whose status is being updated.</param>
        /// <param name="isActive">The new status to set for the user (true for active, false for inactive).</param>
        /// <returns>
        /// Returns:
        /// 1 if the user is a reporting manager and cannot be deactivated.
        /// 2 if the status is updated successfully.
        /// -1 if an error occurs during the update process.
        /// </returns>
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

        /// <summary>
        /// Deletes a user by their ID. If the user is found, they will be removed from the system.
        /// </summary>
        /// <param name="userId">The ID of the user to be deleted.</param>
        /// <returns>
        /// Returns true if the user is successfully deleted, otherwise throws an exception.
        /// </returns>
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

        /// <summary>
        /// Adds a new contact based on the provided model and returns the result as a JSON response.
        /// </summary>
        /// <param name="model">The contact information to be added.</param>
        /// <returns>
        /// A JSON result containing the outcome of adding the contact.
        /// </returns>
        public JsonResult AddContact(ContactDTO model)
        {
            return Json(_IUser.AddContact(model));
        }

        /// <summary>
        /// Adds a new file attachment based on the provided model and returns the result as a JSON response.
        /// </summary>
        /// <param name="File">The file attachment information to be added.</param>
        /// <returns>
        /// A JSON result containing the outcome of adding the file.
        /// </returns>
        public JsonResult AddFile(AttachmentDTO File)
        {
            return Json(_IUser.AddFile(File));
        }
    }
}
