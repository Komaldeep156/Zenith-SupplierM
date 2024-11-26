using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    public class VQWorkFlowController : BaseController
    {
        private readonly IVendorQualificationWorkFlow _IVendorQualificationWorkFlow;
        public VQWorkFlowController(IHttpContextAccessor httpContextAccessor, 
            SignInManager<ApplicationUser> signInManager,
            IVendorQualificationWorkFlow iVendorQualificationWorkFlow) : base(httpContextAccessor, signInManager)
        {
            _IVendorQualificationWorkFlow = iVendorQualificationWorkFlow;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddVQWorkFlow()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddVQWorkFlow(VendorQualificationWorkFlowDTO model)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.SecurityGroupId = Guid.Parse("4E70A2A0-E668-42D4-A503-0A793799B600");
            return Json(_IVendorQualificationWorkFlow.AddVendorQualificationWorkFlow(model, loggedInUserId));
        }

        [HttpGet]
        public async Task<IActionResult> VQWorkFlowList()
        {
            var data = await _IVendorQualificationWorkFlow.GetVendorQualificationWorkFlow();
            return View(data);
        }

        public async Task<ViewResult> VQWorkFlowViewTemplate(Guid vendorQualificationWorkFlowId)
        {
            var data = await _IVendorQualificationWorkFlow.GetVendorQualificationWorkFlowById(vendorQualificationWorkFlowId);
            return View(data);
        }
    }
}
