using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.BLL.Logic;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Zenith.Controllers
{
    public class WorkbenchController : BaseController
    {
        private readonly IVendors _IVendor;
        private readonly IDropdownList _IDropdownList;
        private readonly IVacationRequests _iVacationRequests;
        private readonly IUser _IUser;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public WorkbenchController(IVendors IVendors,
                                IHttpContextAccessor httpContextAccessor,
                                SignInManager<ApplicationUser> signInManager,
                                IWebHostEnvironment webHostEnvironment,
                                IUser iUser, IDropdownList iDropdownList, IVacationRequests iVacationRequests)
      : base(httpContextAccessor, signInManager)
        {
            _IVendor = IVendors;
            _webHostEnvironment = webHostEnvironment;
            _IUser = iUser;
            _IDropdownList = iDropdownList;
            _signInManager = signInManager;
            _iVacationRequests = iVacationRequests;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var rejectReasonDDL= _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.REJECTREASON));
            ViewBag.rejectreason = rejectReasonDDL;
            var data = _IVendor.GetVendors();
            return View(data);
        }

        public IActionResult _VendorApprovalListPartialView(string fieldName, string searchText)
        {
            var lists = _IVendor.SearchVendorList(fieldName, searchText);

            return PartialView(lists);
        }

        public async Task<IActionResult> _VacationRequestsApprovalListPartialView(DateTime? filterStartDate=null, DateTime? filterEndDate=null)
        {
            var rejectReasonDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.REJECTREASON));
            ViewBag.rejectreason = rejectReasonDDL;
            DateTime todayDate=DateTime.Now;
            if (filterStartDate==null)
            filterStartDate = todayDate.AddDays(-60);
            if (filterEndDate == null)
            filterEndDate = todayDate;
            var lists = await _iVacationRequests.GetWorkBenchVacationRequests(Convert.ToDateTime(filterStartDate),Convert.ToDateTime(filterEndDate));
            return PartialView(lists);
        }

        public ViewResult VendorViewTemplate(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            return View(data);
        }

        public async Task<IActionResult> _WorkBenchSummaryPartialView()
        {
            var workBenchSummary = new List<WorkbenchDTO>();
            var pendingWorkStatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.PENDING));
            var WorkingStatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.WORKING));
            var lists = _IVendor.SearchVendorList(string.Empty, string.Empty);
            var VIRRequests = new WorkbenchDTO();
            VIRRequests.ApprovalType = "VIR";
            VIRRequests.PendingStausCount = lists.Count(x => x.StatusId == pendingWorkStatusId);
            VIRRequests.WorkingStausCount = lists.Count(x => x.StatusId == WorkingStatusId);
            VIRRequests.TotalCount = VIRRequests.PendingStausCount+ VIRRequests.WorkingStausCount;
            workBenchSummary.Add(VIRRequests);

            var VQRRequests = new WorkbenchDTO();
            VQRRequests.ApprovalType = "VQR";
            VQRRequests.PendingStausCount = 5;
            VQRRequests.WorkingStausCount = 3;
            VQRRequests.TotalCount = VQRRequests.PendingStausCount + VQRRequests.WorkingStausCount;
            workBenchSummary.Add(VQRRequests);

            var vacationRequests = await _iVacationRequests.GetVacationRequests();
            var VCRRequests = new WorkbenchDTO();
            VCRRequests.ApprovalType = "User Approvals";
            VCRRequests.PendingStausCount = vacationRequests.Count(x => x.StatusId == pendingWorkStatusId);
            VCRRequests.WorkingStausCount = vacationRequests.Count(x => x.StatusId == WorkingStatusId);
            VCRRequests.TotalCount = VCRRequests.PendingStausCount + VCRRequests.WorkingStausCount;

            workBenchSummary.Add(VCRRequests);
            return PartialView(workBenchSummary);
        }

        [HttpPost]
        public JsonResult deleteVendors([FromBody] List<Guid> selectedVendorGuids)
        {
            // Process the received GUIDs
           var isSuccess= _IVendor.DeleteVendors(selectedVendorGuids);
            // Return a success response
            return Json(new { success = isSuccess, message = "Data received successfully" });
        }

        public ViewResult VendorDetails(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            return View(data);
        }

        [HttpPost]
        public async Task<string> UpdateVendor(updateVendorDTO model)
        {
            try
            {
                return await _IVendor.UpdateVendor(model);
            }
            catch (Exception )
            {
                return  "Failed";
            }
        }

        [HttpPost]
        public async Task<string> UpdateVacationRequests(VacationRequestsDTO model)
        {
            try
            {
                return await _iVacationRequests.UpdateVacationRequests(model);
            }
            catch (Exception)
            {
                return "Failed";
            }
        }

        public async Task<bool> UpdateVendorCriticalNonCritical(Guid vendorId,bool isVendorCritical)
        {
            try
            {
                return await _IVendor.UpdateVendorCriticalNonCritical(vendorId, isVendorCritical);
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
