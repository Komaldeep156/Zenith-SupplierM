using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

namespace Zenith.Controllers
{
    [Authorize]
    public class VQWorkFlowController : BaseController
    {
        private readonly IVendorQualificationWorkFlow _IVendorQualificationWorkFlow;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWorkFlows _workFlows;
        private readonly IVendorQualificationWorkFlowExecution _vendorQualificationWorkFlowExecution;
        public VQWorkFlowController(IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            IVendorQualificationWorkFlow iVendorQualificationWorkFlow,
            RoleManager<IdentityRole> roleManager,
            IWorkFlows workFlows,
            IVendorQualificationWorkFlowExecution vendorQualificationWorkFlowExecution) : base(httpContextAccessor, signInManager)
        {
            _IVendorQualificationWorkFlow = iVendorQualificationWorkFlow;
            _roleManager = roleManager;
            _workFlows = workFlows;
            _vendorQualificationWorkFlowExecution = vendorQualificationWorkFlowExecution;
        }

        /// <summary>
        /// Adds a new Vendor Qualification WorkFlow based on the provided model and the logged-in user's ID,
        /// and returns the result as a JSON response.
        /// </summary>
        /// <param name="model">The Vendor Qualification WorkFlow data to be added.</param>
        /// <returns>
        /// A JSON result containing the outcome of adding the Vendor Qualification WorkFlow.
        /// </returns>
        [HttpPost]
        public IActionResult AddVQWorkFlow(VendorQualificationWorkFlowDTO model)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.SecurityGroupId = Guid.Parse("4E70A2A0-E668-42D4-A503-0A793799B600");
            return Json(_IVendorQualificationWorkFlow.AddVendorQualificationWorkFlow(model, loggedInUserId));
        }

        /// <summary>
        /// Retrieves the list of Vendor Qualification WorkFlow steps and associated data based on the provided WorkFlow ID,
        /// then returns the data along with the WorkFlow steps and name to the view.
        /// </summary>
        /// <param name="workFlowId">The unique identifier of the WorkFlow (default is empty GUID).</param>
        /// <returns>
        /// A view displaying the list of Vendor Qualification WorkFlow data along with WorkFlow steps and name.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> VQWorkFlowList(Guid workFlowId = default)
        {
            var workFlowSteps = Enum.GetValues(typeof(WorkFlowEnum))
                .Cast<WorkFlowEnum>()
                .Select(e => new
                {
                    Id = e.ToString(),
                    Name = e.GetStringValue()
                })
                .ToList();

            var workFlow = await _workFlows.GetWorkFlowById(workFlowId);
            ViewBag.WorkFlowStpes = workFlowSteps;
            ViewBag.WorkFlowName = workFlow != null ? workFlow.Name : "";
            ViewBag.workFlowId = workFlowId;
            var data = await _IVendorQualificationWorkFlow.GetVendorQualificationWorkFlow(workFlowId);
            return View(data);
        }

        /// <summary>
        /// Retrieves the detailed view of a specific Vendor Qualification WorkFlow based on the provided ID,
        /// and provides additional data such as role information and WorkFlow steps for display in the view.
        /// </summary>
        /// <param name="vendorQualificationWorkFlowId">The unique identifier of the Vendor Qualification WorkFlow.</param>
        /// <returns>
        /// A view containing the details of the Vendor Qualification WorkFlow along with available role information
        /// and WorkFlow steps.
        /// </returns>
        public async Task<ViewResult> VQWorkFlowViewDetail(Guid vendorQualificationWorkFlowId)
        {
            var data = await _IVendorQualificationWorkFlow.GetVendorQualificationWorkFlowById(vendorQualificationWorkFlowId);
            ViewBag.RoleId = _roleManager.Roles.ToList();
            ViewBag.SecurityGroup = "";
            var workFlowSteps = Enum.GetValues(typeof(WorkFlowEnum))
                .Cast<WorkFlowEnum>()
                .Select(e => new
                {
                    Id = e.ToString(), // Enum name as value
                    Name = e.GetStringValue() // Custom string value
                })
                .ToList();

            ViewBag.WorkFlowStpes = workFlowSteps;

            return View(data);
        }

        /// <summary>
        /// Deletes selected Vendor Qualification WorkFlows based on the provided list of GUIDs, and returns 
        /// the status of the deletion process, including details on any WorkFlows that could not be deleted.
        /// </summary>
        /// <param name="selectedUserGuids">A list of GUIDs representing the Vendor Qualification WorkFlows to be deleted.</param>
        /// <returns>
        /// A JSON response indicating whether the deletion was successful, partially successful, or failed,
        /// along with the names of any WorkFlows that could not be deleted.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DeleteVQWorkFlow([FromBody] List<Guid> selectedUserGuids)
        {
            try
            {
                var result = await _IVendorQualificationWorkFlow.DeleteVendorQualificationWorkFlow(selectedUserGuids);
                return Ok(new
                {
                    isSuccess = result.isSuccess && result.notDeletedVQWorkFlowNames.Count == 0,
                    PartiallySuccess = result.isSuccess && result.notDeletedVQWorkFlowNames.Count > 0,
                    notDeletedVQWorkFlowNames = result.notDeletedVQWorkFlowNames
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { IsSuccess = false, Message = "An error occurred." });
            }
        }

        /// <summary>
        /// Updates the Vendor Qualification WorkFlow with the given model. If the update is successful, it returns a success message.
        /// If the data provided is invalid, it returns an error message.
        /// </summary>
        /// <param name="model">The VendorQualificationWorkFlowDTO model containing the updated details of the Vendor Qualification WorkFlow.</param>
        /// <returns>
        /// A JSON response indicating whether the update was successful or if there was an error due to invalid data.
        /// </returns>
        [HttpPost]
        public async Task<JsonResult> UpdateVQWorkFlow(VendorQualificationWorkFlowDTO model)
        {
            try
            {
                model.SecurityGroupId = Guid.Parse("4E70A2A0-E668-42D4-A503-0A793799B600");
                if (await _IVendorQualificationWorkFlow.UpdateVendorQualificationWorkFlow(model))
                {
                    return new JsonResult(new { responseCode = 2, SuccessResponse = "Successfully Update Record." });

                }
                return new JsonResult(new { ResponseCode = 1, Response = "Please Ennter Valied Data." });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// Updates the execution status of the Vendor Qualification WorkFlow from the workbench using the provided model.
        /// If the update is successful, a success message is returned. Otherwise, an error message indicating invalid data is returned.
        /// </summary>
        /// <param name="model">The VendorQualificationWorkFlowExecutionDTO model containing the details of the work flow execution status.</param>
        /// <returns>
        /// A JSON response indicating whether the update was successful or if the data was invalid.
        /// </returns>
        [HttpPost]
        public async Task<JsonResult> UpdateVQWorkFlowExecutionStatus(VendorQualificationWorkFlowExecutionDTO model)
        {
            try
            {
                if (await _vendorQualificationWorkFlowExecution.UpdateVendorQualificationWorkFlowExecutionStatusFromWorkBench(model))
                {
                    return new JsonResult(new { responseCode = 2, SuccessResponse = "Successfully Update Record." });

                }
                return new JsonResult(new { ResponseCode = 1, Response = "Please Ennter Valied Data." });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
