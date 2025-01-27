using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;

namespace Zenith.Controllers
{
    [Authorize]
    public class WorkFlowsController : Controller
    {
        private readonly IWorkFlows _workFlows;

        public WorkFlowsController(IWorkFlows workFlows)
        {
            _workFlows = workFlows;
        }

        /// <summary>
        /// This method returns the view for the Index page. It is typically used to render the default landing page of the application.
        /// </summary>
        /// <returns>Returns the view associated with the Index action.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This method returns the view for adding a new workflow. It is typically used to render the page where users can add workflow details.
        /// </summary>
        /// <returns>Returns the view associated with the AddWorkFlow action.</returns>
        [HttpGet]
        public IActionResult AddWorkFlow()
        {
            return View();
        }

        /// <summary>
        /// This method handles the submission of a new workflow. It receives the workflow data from the user, processes it, and returns a JSON response indicating whether the workflow was created successfully.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the new workflow to be created.</param>
        /// <returns>Returns a JSON response with the result of the workflow creation process.</returns>
        [HttpPost]
        public IActionResult AddWorkFlow(WorkFlowsDTO model)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Json(_workFlows.CreateWorkFlow(model, loggedInUserId));
        }

        /// <summary>
        /// This method retrieves a list of workflows and returns the view to display them. It fetches the data asynchronously and passes it to the view for rendering.
        /// </summary>
        /// <returns>Returns the view displaying the list of workflows.</returns>
        [HttpGet]
        public async Task<IActionResult> WorkFlowList()
        {
            var data = await _workFlows.GetWorkFlows();
            return View(data);
        }

        /// <summary>
        /// This method retrieves the details of a specific workflow by its ID and returns the view to display the details.
        /// </summary>
        /// <param name="WorkFlowId">The unique identifier of the workflow to retrieve.</param>
        /// <returns>Returns the view displaying the details of the specified workflow.</returns>
        public async Task<ViewResult> WorkFlowViewDetail(Guid WorkFlowId)
        {
            var data = await _workFlows.GetWorkFlowById(WorkFlowId);
            return View(data);
        }

        /// <summary>
        /// This method handles the deletion of selected workflows. It receives a list of workflow IDs to be deleted, processes the request, and returns a JSON response indicating the success or failure of the operation.
        /// </summary>
        /// <param name="selectedWorkFlowGuids">A list of unique identifiers for the workflows to be deleted.</param>
        /// <returns>Returns a JSON response indicating whether the deletion was successful, partially successful, or if any workflows were not deleted.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteWorkFlow([FromBody] List<Guid> selectedWorkFlowGuids)
        {
            try
            {
                var result = await _workFlows.DeleteWorkFlows(selectedWorkFlowGuids);

                return Ok(new
                {
                    IsSuccess = result.isSuccess && result.notDeletedWorkFlowNames.Count == 0,
                    PartiallySuccess = result.isSuccess && result.notDeletedWorkFlowNames.Count > 0,
                    NotDeletedWorkFlows = result.notDeletedWorkFlowNames
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { IsSuccess = false, Message = "An error occurred." });
            }
        }

        /// <summary>
        /// This method handles the update of an existing workflow. It receives the updated workflow data, processes the update, and returns a JSON response indicating whether the update was successful or if there was an issue with the data provided.
        /// </summary>
        /// <param name="model">The data transfer object containing the updated details of the workflow.</param>
        /// <returns>Returns a JSON response indicating whether the workflow was successfully updated or if invalid data was provided.</returns>
        [HttpPost]
        public async Task<JsonResult> UpdateWorkFlow(WorkFlowsDTO model)
        {
            try
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (await _workFlows.UpdateWorkFlow(model, loggedInUserId))
                {
                    return new JsonResult(new { responseCode = 2, SuccessResponse = "Successfully Update Record." });

                }
                return new JsonResult(new { ResponseCode = 1, Response = "Please Enters Valid Data." });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
