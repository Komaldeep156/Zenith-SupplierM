using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

namespace Zenith.Controllers
{
    [Authorize]
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
        private readonly IVendorQualificationWorkFlowExecution _vendorQualificationWorkFlowExecution;
        public WorkbenchController(IVendors IVendors,
                                IHttpContextAccessor httpContextAccessor,
                                SignInManager<ApplicationUser> signInManager,
                                IWebHostEnvironment webHostEnvironment,
                                IUser iUser, IDropdownList iDropdownList, IVacationRequests iVacationRequests, UserManager<ApplicationUser> userManager, IDelegationRequests iDelegationRequests, IVendorQualificationWorkFlowExecution vendorQualificationWorkFlowExecution)
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
            _vendorQualificationWorkFlowExecution = vendorQualificationWorkFlowExecution;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await _IVendor.GetVendors(loggedInUserId);

            var codeArray = new[] { "PND", "WORKING" };
            data.WorkStatusDDL = _IDropdownList.GetDropdownListByArry(codeArray);
            data.RejectReasonDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.REJECTREASON));
            data.DelegateUserListDDL = (await GetUsersInManagerRoleAsync()).Where(x => x.Id != loggedInUserId).ToList();

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> OfficerWorkbench()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await _IVendor.GetVendors(loggedInUserId);

            var codeArray = new[] { "PND", "WORKING" };
            data.WorkStatusDDL = _IDropdownList.GetDropdownListByArry(codeArray);
            data.DelegateUserListDDL  = await GetUsersInManagerRoleAsync();
            data.RejectReasonDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.REASSIGNREASONS));
            return View(data);
        }

        public async Task<List<ApplicationUser>> GetUsersInManagerRoleAsync()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(RolesEnum.VENDOR_MANAGER.GetStringValue());
            return usersInRole.ToList();
        }

        //For serching
        public async Task<IActionResult> _VendorApprovalListPartialView(string fieldName, string searchText)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lists = await _IVendor.SearchVendorList(fieldName, searchText, loggedInUserId);

            var codeArray = new[] { "PND", "WORKING" };
            lists.WorkStatusDDL = _IDropdownList.GetDropdownListByArry(codeArray);

            return PartialView(lists);
        }

        //For Searching
        public async Task<IActionResult> _OfficerWorkBenchRequestsList(string fieldName, string searchText)
        {

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lists = await _IVendor.SearchVendorList(fieldName, searchText, loggedInUserId);

            var codeArray = new[] { "PND", "WORKING" };
            lists.WorkStatusDDL = _IDropdownList.GetDropdownListByArry(codeArray);

            return PartialView(lists);
        }

        public async Task<IActionResult> _ManageDelegationRequestsList(string fieldName, string searchText)
        {
            var lists = await _iDelegationRequests.GetDelegationRequests(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return PartialView(lists);
        }

        public async Task<IActionResult> _VacationRequestsApprovalListPartialView(DateTime? filterStartDate = null, DateTime? filterEndDate = null)
        {
            var rejectReasonDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.REJECTREASON));
            ViewBag.rejectreason = rejectReasonDDL;


            DateTime todayDate = DateTime.Now;
            if (filterStartDate == null)
                filterStartDate = todayDate.AddDays(-60);
            if (filterEndDate == null)
                filterEndDate = todayDate;
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lists = await _iVacationRequests.GetWorkBenchVacationRequests(Convert.ToDateTime(filterStartDate), Convert.ToDateTime(filterEndDate), loggedInUserId);

            var codeArray = new[] { "PND", "WORKING" };
            var workStatus = _IDropdownList.GetDropdownListByArry(codeArray);
            
            var model = new VendorViewModel
            {
                VacationList = lists,
                RejectReasonDDL = rejectReasonDDL,
                WorkStatusDDL = workStatus
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateVacationRequestStatusStatus(VacationRequests model)
        {
            try
            {
                if (await _iVacationRequests.UpdateVacationRequestStatus(model))
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

        public async Task<ViewResult> VendorViewTemplate(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            var user = await _IUser.GetUserByIdAsync(data.CreatedBy);
            var departmnet = await _IDropdownList.GetDropDownValuById(user.DepartmentId ?? Guid.Empty);

            data.Department = departmnet;
            data.Position = user.RoleName;
            data.CreatedBy = user.FullName;
            return View(data);
        }
        public async Task<ViewResult> UpdateVendorDetails(Guid VendorsInitializationFormId)
        {
            var codeArray = new[] { "NEWVEN" };
            var RequestType = _IDropdownList.GetDropdownListByArry(codeArray);

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _IUser.GetUserByIdAsync(loggedInUserId);
            var departmnet = await _IDropdownList.GetDropDownValuById(user.DepartmentId ?? Guid.Empty);

            var data = _IVendor.GetVendorById(VendorsInitializationFormId);

            //For Requested By
            var requestedByUser = await _IUser.GetUserByIdAsync(data.RequestedBy.ToString());
            data.RequestedByDepartment = await _IDropdownList.GetDropDownValuById(requestedByUser.DepartmentId ?? Guid.Empty);
            data.RequestedByPosition = requestedByUser.RoleName;
            data.RequestedByName = requestedByUser.FullName;
            data.RequestedByEmail = requestedByUser.Email;

            var model = new VendorCreateModel
            {
                UsersList = _IUser.GetUsers(),
                getVendorsListDTO = data,
                CreatedBy = user,
                RequestType = RequestType,
                Position = user.RoleName,
                Department = departmnet,
                Email = user.Email,
            };
            return View(model);
        }

        public async Task<JsonResult>GetCreatedByInfo(Guid userId)
        {
            if (userId == Guid.Empty)
                return new JsonResult(new {responseCode = 0});

            var user = await _IUser.GetUserByIdAsync(userId.ToString());
            var departmnet = await _IDropdownList.GetDropDownValuById(user.DepartmentId ?? Guid.Empty);

            var data = new
            {
                Department = departmnet,
                Position = user.RoleName,
                Email = user.Email
            };
            return new JsonResult(new { responseCode = 1, data = data});
        }

        [HttpPost]
        public async Task<JsonResult> UpdateVendorDetails(VendorDTO model)
        {
            var vendor = new VendorsInitializationForm
            {
                SupplierName = model.SupplierName,
                SupplierCountryId = model.SupplierCountryId,
                BusinessRegistrationNo = model.BusinessRegistrationNo,
            };
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await  _IVendor.UpdateVendorDetails(model, loggedInUserId);
            if (data)
            {
                return new JsonResult(new { responseCode = 0, SuccessResponse = "Successfully Update Record." });
            }
            return new JsonResult(new { responseCode = 1, SuccessResponse = "Please Try Again." });
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
            var VIRPendingStatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRPND));
            var VQFPendingStatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VQFPND));
            var vIRDelegateStatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.DLR));
            var vIRUnderReviewStatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRUR));

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vendor = await _IVendor.SearchVendorList(string.Empty, string.Empty, loggedInUserId);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "Vendor Officer")
            {
                var VIRRequests = new WorkbenchDTO();
                VIRRequests.ApprovalType = "VIR";
                VIRRequests.PendingStausCount = vendor.Vendors.Count(x => x.StatusId == VIRPendingStatusId && x.WorkStatusId == pendingWorkStatusId);
                VIRRequests.WorkingStausCount = vendor.Vendors.Count(x => x.StatusId == VIRPendingStatusId && x.WorkStatusId == WorkingStatusId);
                VIRRequests.DelegateRequested = vendor.Vendors.Count(x => x.StatusId == VIRPendingStatusId && x.WorkStatusId == vIRDelegateStatusId);
                VIRRequests.TotalCount = VIRRequests.PendingStausCount + VIRRequests.WorkingStausCount + VIRRequests.DelegateRequested;
                VIRRequests.UserRole = userRole;
                workBenchSummary.Add(VIRRequests);

                DateTime todayDate = DateTime.Now;
                var filterStartDate = todayDate.AddDays(-60);
                var filterEndDate = todayDate;
                var vacationRequests = await _iVacationRequests.GetWorkBenchVacationRequests(Convert.ToDateTime(filterStartDate), Convert.ToDateTime(filterEndDate), loggedInUserId);
                var VCRRequests = new WorkbenchDTO();
                VCRRequests.ApprovalType = "User Approvals";
                VCRRequests.PendingStausCount = vacationRequests.Count(x => x.StatusId == pendingWorkStatusId);
                VCRRequests.WorkingStausCount = vacationRequests.Count(x => x.StatusId == WorkingStatusId);
                VCRRequests.DelegateRequested = vacationRequests.Count(x => x.StatusId == vIRDelegateStatusId);
                VCRRequests.TotalCount = VCRRequests.PendingStausCount + VCRRequests.WorkingStausCount + VCRRequests.DelegateRequested;
                VCRRequests.UserRole = userRole;
                workBenchSummary.Add(VCRRequests);
            }
            else if(userRole == "Vendor Officer")
            {
                var VIRRequests = new WorkbenchDTO();
                VIRRequests.ApprovalType = "VIR";
                VIRRequests.PendingStausCount = vendor.Vendors.Count(x => x.StatusId == vIRUnderReviewStatusId && x.WorkStatusId == pendingWorkStatusId);
                VIRRequests.WorkingStausCount = vendor.Vendors.Count(x => x.StatusId == vIRUnderReviewStatusId && x.WorkStatusId == WorkingStatusId);
                VIRRequests.DelegateRequested = vendor.Vendors.Count(x => x.StatusId == vIRUnderReviewStatusId && x.WorkStatusId == vIRDelegateStatusId);
                VIRRequests.TotalCount = VIRRequests.PendingStausCount + VIRRequests.WorkingStausCount + VIRRequests.DelegateRequested;
                VIRRequests.UserRole = userRole;
                workBenchSummary.Add(VIRRequests);
            }

            var VQRRequests = new WorkbenchDTO();
            VQRRequests.ApprovalType = "VQR";
            VQRRequests.PendingStausCount = vendor.Vendors.Count(x => x.StatusId == VQFPendingStatusId && x.WorkStatusId == pendingWorkStatusId);
            VQRRequests.WorkingStausCount = vendor.Vendors.Count(x => x.StatusId == VQFPendingStatusId && x.WorkStatusId == WorkingStatusId);
            VQRRequests.DelegateRequested = vendor.Vendors.Count(x => x.StatusId == VQFPendingStatusId && x.WorkStatusId == vIRDelegateStatusId);
            VQRRequests.TotalCount = VQRRequests.PendingStausCount + VQRRequests.WorkingStausCount + VQRRequests.DelegateRequested;
            VQRRequests.UserRole = userRole;
            workBenchSummary.Add(VQRRequests);

            return PartialView(workBenchSummary);
        }

        [HttpPost]
        public JsonResult deleteVendors([FromBody] List<Guid> selectedVendorGuids)
        {
            // Process the received GUIDs
            var isSuccess = _IVendor.DeleteVendors(selectedVendorGuids);
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
            catch (Exception)
            {
                return "Failed";
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

        public async Task<bool> UpdateVendorCriticalNonCritical(Guid vendorId, bool isVendorCritical)
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

                if (!string.IsNullOrEmpty(loggedInUserId) && delegateRequestDTO != null && !string.IsNullOrEmpty(delegateRequestDTO.CommaSprtdRecordIds))
                {
                    List<string> rcrdIds = delegateRequestDTO.CommaSprtdRecordIds
                                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .ToList();
                    if (delegateRequestDTO.RecordTypeCd == ApprovalTypeEnum.VIR.GetStringValue())
                    {
                        await _vendorQualificationWorkFlowExecution.UpdateVendorQualificationWorkFlowExecutionStatus(rcrdIds, DropDownValuesEnum.DelegateRequested.GetStringValue(), loggedInUserId);
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

        public async Task<bool> AcceptOrRejectDelegateRequest(Guid delegateRequestId, bool isDelegationReqAccepted)
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
