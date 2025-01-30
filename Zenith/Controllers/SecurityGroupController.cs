using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SecurityGroupController : BaseController
    {
        private readonly ISecurityGroup _securityGroup;
        private readonly IFields _fields;
        private readonly ISecurityGroupUsersLogic _securityGroupUsersLogic;

        public SecurityGroupController(IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            ISecurityGroup securityGroup,
            IFields fields,
            ISecurityGroupUsersLogic securityGroupUsersLogic) : base(httpContextAccessor, signInManager)
        {
            _securityGroup = securityGroup;
            _fields = fields;
            _securityGroupUsersLogic = securityGroupUsersLogic;
        }

        /// <summary>
        /// Retrieves a list of all security groups asynchronously and returns the view displaying the list.
        /// </summary>
        /// <returns>view with the list of security groups.</returns>
        public async Task<IActionResult> Index()
        {
            var list = await _securityGroup.GetAllSecurityGroups();
            return View(list);
        }

        /// <summary>
        /// Searches for security groups based on the specified field name and search text, 
        /// retrieves the matching results asynchronously, and returns a partial view with the list.
        /// </summary>
        /// <param name="fieldName">The name of the field to search on.</param>
        /// <param name="searchText">The text to search for within the specified field.</param>
        /// <returns>A PartialViewResult displaying the filtered list of security groups.</returns>
        public async Task<IActionResult> SearchSecurityGroupList(string fieldName, string searchText)
        {
            var list = await _securityGroup.GetAllSecurityGroups(null, fieldName, searchText);

            return PartialView("_SecurityGroupList", list);
        }

        /// <summary>
        /// Retrieves a list of active security groups asynchronously, formats them into a JSON object, 
        /// and returns the result. Only groups with an active status are included.
        /// </summary>
        /// <returns>A JsonResult containing a formatted list of active security groups with their ID and Name.</returns>

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSecurityGroupJsonList()
        {
            var securityGroups = await _securityGroup.GetAllSecurityGroups(); ; // Fetch users from the IUser service
            var activeSecurityGroups = securityGroups.Where(x => x.IsActive).Select(x => new { x.Id, x.Name }).ToList();
            return new JsonResult(activeSecurityGroups, new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            });
        }

        /// <summary>
        /// Initializes the Add Security Group view  and Fields property with a list of all fields retrieved asynchronously
        /// </summary>
        /// <returns>view with the initialized model.</returns>
        [HttpGet]
        public async Task<IActionResult> AddSecurityGroup()
        {
            var model = new SecurityGroupsDTO()
            {
                Fields = await _fields.GetAllFields()
            };

            return View(model);
        }

        /// <summary>
        /// Adds a new security group and assigns users to it.
        /// </summary>
        /// <param name="model">The security group data transfer object containing the details of the new security group.</param>
        /// <returns>A JSON response indicating success or failure.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the model is null.</exception>
        [HttpPost]
        public async Task<IActionResult> AddSecurityGroup(SecurityGroupsDTO model)
        {
            var duplicateNameAndCode = await _securityGroup.IsDuplicateSecurityGroup(model);
            if (duplicateNameAndCode != 0)
            {
                return new JsonResult(new { ResponseCode = duplicateNameAndCode, message = "Please reenter data." });
            }

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.CreatedBy = loginUserId;
            var securityGroupId = await _securityGroup.AddSecurityGroup(model);

            if (model.AssignedUserIds != null)
            {
                foreach (var userId in model.AssignedUserIds)
                {
                    var securityGroupUsers = new SecurityGroupUsersDTO
                    {
                        UserId = userId.ToString(),
                        SecurityGroupId = securityGroupId,
                        CreatedBy = loginUserId,
                        CreatedOn = DateTime.Now,
                    };
                    await _securityGroupUsersLogic.AddSecurityGroupUsers(securityGroupUsers);
                }
            }

            return new JsonResult(new { ResponseCode = 0, message = "Security group is created successfully." });
        }

        /// <summary>
        /// Copy of existing security group. then saves same security group along with its assigned users.
        /// </summary>
        /// <param name="model">The SecurityGroupsDTO model containing security group details and assigned user IDs.</param>
        /// <returns>
        /// A JsonResult indicating the operation's success or failure, including an appropriate response code 
        /// and message.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CopySecurityGroup([FromBody] List<Guid> selectedSecurityGroupGuids)
        {
            try
            {
                var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _securityGroup.CopySecurityGroup(selectedSecurityGroupGuids, loginUserId);

                return Ok(new
                {
                    isSuccess = result.isSuccess && result.notCopySecurityGroupNames.Count == 0,
                    PartiallySuccess = result.isSuccess && result.notCopySecurityGroupNames.Count > 0,
                    notCopySecurityGroupNames = result.notCopySecurityGroupNames,
                    copySecurityGroupId = result.copiedSecurityGroupId,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { IsSuccess = false, Message = "An error occurred." });
            }
        }

        /// <summary>
        /// Deletes a list of security groups based on their GUIDs. Returns the operation's success status, 
        /// including details about any security groups that could not be deleted.
        /// </summary>
        /// <param name="selectedSecurityGroupGuids">A list of GUIDs representing the security groups to be deleted.</param>
        /// <returns>
        /// An OkObjectResult with a response indicating whether the operation was fully or partially successful, 
        /// and a list of security group names that could not be deleted. Returns a 500 status code with an error message if an exception occurs.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DeleteSecurityGroup([FromBody] List<Guid> selectedSecurityGroupGuids)
        {
            try
            {
                var result = await _securityGroup.DeleteSecurityGroup(selectedSecurityGroupGuids);
                return Ok(new
                {
                    isSuccess = result.isSuccess && result.notDeletedSecurityGroupNames.Count == 0,
                    PartiallySuccess = result.isSuccess && result.notDeletedSecurityGroupNames.Count > 0,
                    notDeletedSecurityGroupNames = result.notDeletedSecurityGroupNames
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { IsSuccess = false, Message = "An error occurred." });
            }
        }

        /// <summary>
        /// Updates the status of selected security groups to either active or inactive based on the provided status.  
        /// Returns the operation's success status, including details about any security groups that could not be updated.
        /// </summary>
        /// <param name="selectedSecurityGroupGuids">A list of GUIDs representing the security groups to update.</param>
        /// <param name="isActive">A boolean indicating whether to activate or deactivate the selected security groups.</param>
        /// <returns>
        /// An OkObjectResult with a response indicating whether the operation was fully or partially successful,  
        /// and a list of security group names that could not be updated. Returns a 500 status code with an error message if an exception occurs.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSecurityGroupStatus(List<Guid> selectedSecurityGroupGuids, bool isActive)
        {
            try
            {
                var result = await _securityGroup.UpdateSecurityGroupToActive(selectedSecurityGroupGuids, isActive);
                return Ok(new
                {
                    isSuccess = result.isSuccess && result.notUpdatedSecurityGroupNames.Count == 0,
                    PartiallySuccess = result.isSuccess && result.notUpdatedSecurityGroupNames.Count > 0,
                    notUpdatedSecurityGroupNames = result.notUpdatedSecurityGroupNames
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { IsSuccess = false, Message = "An error occurred." });
            }
        }

        /// <summary>
        /// Retrieves the details of a specific security group by its ID and renders the view with the group's data.  
        /// </summary>
        /// <param name="securityGroupId">The GUID of the security group to retrieve.</param>
        /// <returns>view with the details of the specified security group.</returns>
        public async Task<IActionResult> SecurityGroupTemplate(Guid securityGroupId)
        {
            var list = await _securityGroup.GetAllSecurityGroups(securityGroupId, null, null);
            return View(list.FirstOrDefault());
        }

        /// <summary>
        /// Retrieves the details of a specific security group by its ID for editing or viewing purposes.  
        /// Renders the view with the retrieved security group data.
        /// </summary>
        /// <param name="securityGroupId">The GUID of the security group to retrieve.</param>
        /// <returns>view with the security group details.</returns>
        public async Task<IActionResult> EditAndViewSecurityGroup(Guid securityGroupId)
        {
            var model = await _securityGroup.GetSecurityGroupsById(securityGroupId);
            return View(model);
        }

        /// <summary>
        /// Handles the editing and updating of a security group's details. It updates the security group information, 
        /// removes existing user assignments, and assigns new users to the security group if specified.
        /// </summary>
        /// <param name="model">The SecurityGroupsDTO model containing the updated security group details and assigned user IDs.</param>
        /// <returns>A JsonResult indicating the operation's success or failure, along with a message.</returns>
        [HttpPost]
        public async Task<IActionResult> EditAndViewSecurityGroup(SecurityGroupsDTO model)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.CreatedBy = loginUserId;
            var result = await _securityGroup.UpdateSecurityGroup(model);

            await _securityGroupUsersLogic.RemoveSecurityGroupUsers(model.Id);

            if (model.AssignedUserIds != null)
            {
                foreach (var userId in model.AssignedUserIds)
                {
                    var securityGroupUsers = new SecurityGroupUsersDTO
                    {
                        UserId = userId.ToString(),
                        SecurityGroupId = model.Id,
                        CreatedBy = loginUserId,
                        CreatedOn = DateTime.Now,
                    };
                    await _securityGroupUsersLogic.AddSecurityGroupUsers(securityGroupUsers);
                }
            }

            return new JsonResult(new { ResponseCode = result.isSuccess, message = result.message });
        }
    }
}
