using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

namespace Zenith.Controllers
{
    public class WorkbenchController : BaseController
    {
        private readonly IVendors _IVendor;
        private readonly IDropdownList _IDropdownList;
        private readonly IVacationRequests _iVacationRequests;
        private readonly IDelegationRequests _iDelegationRequests;
        private readonly IUser _IUser;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public WorkbenchController(IVendors IVendors,
                                IHttpContextAccessor httpContextAccessor,
                                SignInManager<ApplicationUser> signInManager,
                                IWebHostEnvironment webHostEnvironment,
                                IUser iUser, IDropdownList iDropdownList, IVacationRequests iVacationRequests, UserManager<ApplicationUser> userManager, IDelegationRequests iDelegationRequests)
      : base(httpContextAccessor, signInManager)
        {
            _IVendor = IVendors;
            _webHostEnvironment = webHostEnvironment;
            _IUser = iUser;
            _IDropdownList = iDropdownList;
            _signInManager = signInManager;
            _iVacationRequests = iVacationRequests;
            _userManager = userManager;
            _iDelegationRequests = iDelegationRequests;
        }

        [HttpGet] 
        public async Task<IActionResult> Index()
        {
            var rejectReasonDDL= _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.REJECTREASON));
            ViewBag.rejectreason = rejectReasonDDL;

            var codeArray = new[] { "PND", "DG", "WK" };
            var dropDownValues = _IDropdownList.GetDropdownListByArry(codeArray);
            ViewBag.WorkStatus = dropDownValues;

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.DelegateUserListDDL = (await GetUsersInManagerRoleAsync()).Where(x=>x.Id != loggedInUserId);
           
            var data = _IVendor.GetVendors(loggedInUserId);
            return View(data);
        }

        [HttpGet] 
        public async Task<IActionResult> OfficerWorkbench()
        {
            var re_AssignReasonDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.REASSIGNREASONS));
            ViewBag.re_AssignReasonDDL = re_AssignReasonDDL;
            ViewBag.DelegateUserListDDL = await GetUsersInManagerRoleAsync();
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = _IVendor.GetVendors(loggedInUserId);
            return View(data);
        }

        public async Task<List<ApplicationUser>> GetUsersInManagerRoleAsync()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(RolesEnum.VENDOR_MANAGER.GetStringValue());
            return usersInRole.ToList();
        }

        public IActionResult _VendorApprovalListPartialView(string fieldName, string searchText)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lists = _IVendor.SearchVendorList(fieldName, searchText, loggedInUserId);

            return PartialView(lists);
        }

        public IActionResult _OfficerWorkBenchRequestsList(string fieldName, string searchText)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lists = _IVendor.SearchVendorList(fieldName, searchText, loggedInUserId);

            return PartialView(lists);
        }

        public async Task<IActionResult> _ManageDelegationRequestsList(string fieldName, string searchText)
        {
            var lists = await _iDelegationRequests.GetDelegationRequests(User.FindFirstValue(ClaimTypes.NameIdentifier));

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
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lists = await _iVacationRequests.GetWorkBenchVacationRequests(Convert.ToDateTime(filterStartDate),Convert.ToDateTime(filterEndDate), loggedInUserId);
            return PartialView(lists);
        }

        public ViewResult VendorViewTemplate(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> VacationView(Guid vacationRequestsId)
        {

            var data = await _iVacationRequests.GetVacationRequestsId(vacationRequestsId);
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
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return await _IVendor.UpdateVendor(model, loggedInUserId);
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

        public async Task<bool> CreateDelegateRequest(CreateDelegateRequestDTO delegateRequestDTO)
        {
            try
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(loggedInUserId) && delegateRequestDTO!=null && !string.IsNullOrEmpty(delegateRequestDTO.CommaSprtdRecordIds))
                {
                    List<string> rcrdIds = delegateRequestDTO.CommaSprtdRecordIds
                                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .ToList();
                    if (delegateRequestDTO.RecordTypeCd== ApprovalTypeEnum.VIR.GetStringValue())
                    {
                        await _IVendor.UpdateVendorStatuses(rcrdIds, DropDownValuesEnum.DelegateRequested.GetStringValue());
                    }
                    else
                    {
                        await _iVacationRequests.UpdateVacationRequestsStatuses(rcrdIds, DropDownValuesEnum.DelegateRequested.GetStringValue());
                    }
                         await _iDelegationRequests.AddNew(delegateRequestDTO, loggedInUserId);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AcceptOrRejectDelegateRequest(Guid delegateRequestId,bool isDelegationReqAccepted)
        {
            try
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _iDelegationRequests.AcceptOrRejectDelegateRequest(delegateRequestId, isDelegationReqAccepted, loggedInUserId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
