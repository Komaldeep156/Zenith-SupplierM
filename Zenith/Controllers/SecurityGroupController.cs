using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;

namespace Zenith.Controllers
{
    [Authorize(Roles = "Admin,Vendor Manager")]
    public class SecurityGroupController : Controller
    {
        private readonly ISecurityGroup _securityGroup;
        private readonly IFields _fields;
        private readonly ISecurityGroupUsersLogic _securityGroupUsersLogic;

        public SecurityGroupController(ISecurityGroup securityGroup, IFields fields, ISecurityGroupUsersLogic securityGroupUsersLogic)
        {
            _securityGroup = securityGroup;
            _fields = fields;
            _securityGroupUsersLogic = securityGroupUsersLogic;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _securityGroup.GetAllSecurityGroups();
            return View(list);
        }

        public async Task<IActionResult> SearchSecurityGroupList(string fieldName, string searchText)
        {
            var list = await _securityGroup.GetAllSecurityGroups(null, fieldName, searchText);

            return PartialView("_SecurityGroupList", list);
        }

        [HttpGet]
        public async Task<IActionResult> AddSecurityGroup()
        {
            var model = new SecurityGroupsDTO()
            {
                Fields = await _fields.GetAllfields()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddSecurityGroup(SecurityGroupsDTO model)
        {
            var duplicateNameAndCode = await _securityGroup.IsDuplicateSecurityGroup(model);
            if (duplicateNameAndCode != 0)
            {
                return new JsonResult( new {ResponseCode = duplicateNameAndCode ,message ="Please reenter data."});
            }

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.CreatedBy = loginUserId;
            var securityGroupId = await _securityGroup.AddSecurityGroup(model);

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
            return new JsonResult(new { ResponseCode = 0, message = "Security group is created successfully." });
        }

        [HttpPost]
        public async Task<IActionResult>CopySecurityGroup([FromBody] List<Guid> selectedScurityGroupGuids)
        {
            try
            {
                var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _securityGroup.CopySecurityGroup(selectedScurityGroupGuids, loginUserId);
               
                return Ok(new
                {
                    isSuccess = result.isSuccess && result.notCopySecurityGroupNames.Count == 0,
                    PartiallySuccess = result.isSuccess && result.notCopySecurityGroupNames.Count > 0,
                    notCopySecurityGroupNames = result.notCopySecurityGroupNames
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { IsSuccess = false, Message = "An error occurred." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSecurityGroup([FromBody] List<Guid> selectedScurityGroupGuids)
        {
            try
            {
                var result = await _securityGroup.DeleteSecurityGroup(selectedScurityGroupGuids);
                return Ok(new
                {
                    isSuccess = result.isSuccess && result.notDeletedSecurityGroupNames.Count == 0,
                    PartiallySuccess = result.isSuccess && result.notDeletedSecurityGroupNames.Count > 0,
                    notDeletedSecurityGroupNames = result.notDeletedSecurityGroupNames
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { IsSuccess = false, Message = "An error occurred." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSecurityGroupStatus(List<Guid> selectedScurityGroupGuids, bool isActive)
        {
            try
            {
                var result = await _securityGroup.UpdateSecurityGroupToActive(selectedScurityGroupGuids, isActive);
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

        public async Task<IActionResult> SecurityGroupTemplate(Guid scurityGroupId)
        {
            var list = await _securityGroup.GetAllSecurityGroups(scurityGroupId, null, null);
            return View(list.FirstOrDefault());
        }
    }
}
