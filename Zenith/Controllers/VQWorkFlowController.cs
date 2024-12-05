using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

namespace Zenith.Controllers
{
    public class VQWorkFlowController : BaseController
    {
        private readonly IVendorQualificationWorkFlow _IVendorQualificationWorkFlow;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWorkFlows _workFlows;
        public VQWorkFlowController(IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            IVendorQualificationWorkFlow iVendorQualificationWorkFlow,
            RoleManager<IdentityRole> roleManager,
            IWorkFlows workFlows) : base(httpContextAccessor, signInManager)
        {
            _IVendorQualificationWorkFlow = iVendorQualificationWorkFlow;
            _roleManager = roleManager;
            _workFlows = workFlows;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult AddVQWorkFlow()
        //{
        //    return View();
        //}

        [HttpPost]
        public IActionResult AddVQWorkFlow(VendorQualificationWorkFlowDTO model)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.SecurityGroupId = Guid.Parse("4E70A2A0-E668-42D4-A503-0A793799B600");
            return Json(_IVendorQualificationWorkFlow.AddVendorQualificationWorkFlow(model, loggedInUserId));
        }

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
            var data = await _IVendorQualificationWorkFlow.GetVendorQualificationWorkFlow(workFlowId);
            return View(data);
        }

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

        [HttpPost]
        public async Task<bool> DeleteVQWorkFlow([FromBody] List<string> selectedUserGuids)
        {
            try
            {
                List<string> canNotDeleteUsers = new List<string>();
                foreach (var workFlowId in selectedUserGuids)
                {
                    await _IVendorQualificationWorkFlow.DeleteVendorQualificationWorkFlow(Guid.Parse(workFlowId));
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

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
    }
}
