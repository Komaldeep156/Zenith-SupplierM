using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;

namespace Zenith.Controllers
{
    public class SecurityGroupController : Controller
    {
        private readonly ISecurityGroup _securityGroup;

        public SecurityGroupController(ISecurityGroup securityGroup)
        {
            _securityGroup = securityGroup;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddScurityGroup(SecurityGroupsDTO model)
        {
            model.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _securityGroup.AddSecurityGroup(model);

            return RedirectToAction("Index");
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
                    notDeletedVQWorkFlowNames = result.notDeletedSecurityGroupNames
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
                    notDeletedVQWorkFlowNames = result.notUpdatedSecurityGroupNames
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { IsSuccess = false, Message = "An error occurred." });
            }
        }

        public async Task<IActionResult> SecurityGroupTemplate(Guid scurityGroupId)
        {
            var list = await _securityGroup.GetAllSecurityGroups(scurityGroupId, null,null);
            return View(list.FirstOrDefault());
        }
    }
}
