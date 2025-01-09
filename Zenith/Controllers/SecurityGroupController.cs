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

        public SecurityGroupController(ISecurityGroup securityGroup, IFields fields)
        {
            _securityGroup = securityGroup;
            _fields = fields;
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
        public async Task<IActionResult> AddScurityGroup(SecurityGroupsDTO model)
        {
            var duplicateNameAndCode = await _securityGroup.IdDucplicateSecurityGroup(model);
            if (duplicateNameAndCode != 0)
            {
                return new JsonResult( new {ResponseCode = duplicateNameAndCode ,message ="Please reenter data."});
            }

            model.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _securityGroup.AddSecurityGroup(model);

            return new JsonResult(new { ResponseCode = 0, message = "Security group is created successfully." });
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
