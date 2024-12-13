using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;

namespace Zenith.Controllers
{
    public class WorkFlowsController : Controller
    {
        private readonly IWorkFlows _workFlows;

        public WorkFlowsController(IWorkFlows workFlows)
        {
            _workFlows = workFlows;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddWorkFlow()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddWorkFlow(WorkFlowsDTO model)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Json(_workFlows.CreateWorkFlow(model, loggedInUserId));
        }

        [HttpGet]
        public async Task<IActionResult> WorkFlowList()
        {
            var data = await _workFlows.GetWorkFlows();
            return View(data);
        }

        public async Task<ViewResult> WorkFlowViewDetail(Guid WorkFlowId)
        {
            var data = await _workFlows.GetWorkFlowById(WorkFlowId);
            return View(data);
        }

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
                return new JsonResult(new { ResponseCode = 1, Response = "Please Ennter Valied Data." });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
