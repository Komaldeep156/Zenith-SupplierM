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

        /// <summary>
        /// Retrieves a list of vendors based on the logged-in user's ID, and populates dropdown lists for work status, reject reasons,
        /// and a delegate user list (excluding the logged-in user). Then, it returns the populated data to the view.
        /// </summary>
        /// <returns>
        /// An IActionResult that represents the populated vendor data, along with the necessary dropdown lists for the view.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await _IVendor.GetVendors(loggedInUserId);

            var codeArray = new[] { "PND", "WORKING" };
            data.WorkStatusDDL = _IDropdownList.GetDropdownListByArray(codeArray);
            data.RejectReasonDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.REJECTREASON));
            data.DelegateUserListDDL = (await GetUsersInManagerRoleAsync()).Where(x => x.Id != loggedInUserId).ToList();

            return View(data);
        }

        /// <summary>
        /// Retrieves the list of vendors for the logged-in user and populates dropdown lists for work status, delegate users, 
        /// and reassignment reasons. Returns the populated data to the view for the officer's workbench.
        /// </summary>
        /// <returns>
        /// An IActionResult that represents the populated vendor data, along with the necessary dropdown lists for the officer's workbench view.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> OfficerWorkbench()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await _IVendor.GetVendors(loggedInUserId);

            var codeArray = new[] { "PND", "WORKING" };
            data.WorkStatusDDL = _IDropdownList.GetDropdownListByArray(codeArray);
            data.DelegateUserListDDL = await GetUsersInManagerRoleAsync();
            data.RejectReasonDDL = _IDropdownList.GetDropdownByName(nameof(DropDownListsEnum.REASSIGNREASONS));
            return View(data);
        }

        /// <summary>
        /// Retrieves a list of users who are assigned the 'Vendor Manager' role.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ApplicationUser"/> objects representing users with the 'Vendor Manager' role.
        /// </returns>
        public async Task<List<ApplicationUser>> GetUsersInManagerRoleAsync()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(RolesEnum.VENDOR_MANAGER.GetStringValue());
            return usersInRole.ToList();
        }

        //For serching
        /// <summary>
        /// Handles the search functionality for the vendor approval list. Filters vendors based on the provided field name and search text.
        /// </summary>
        /// <param name="fieldName">The field name to search on.</param>
        /// <param name="searchText">The text to search for within the specified field.</param>
        /// <returns>
        /// A partial view with the filtered vendor list, along with the work status dropdown list.
        /// </returns>
        public async Task<IActionResult> _VendorApprovalListPartialView(string fieldName, string searchText)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lists = await _IVendor.SearchVendorList(fieldName, searchText, loggedInUserId);

            var codeArray = new[] { "PND", "WORKING" };
            lists.WorkStatusDDL = _IDropdownList.GetDropdownListByArray(codeArray);

            return PartialView(lists);
        }

        //For Searching
        /// <summary>
        /// Handles the search functionality for the officer's workbench requests list. Filters requests based on the provided field name and search text.
        /// </summary>
        /// <param name="fieldName">The field name to search on.</param>
        /// <param name="searchText">The text to search for within the specified field.</param>
        /// <returns>
        /// A partial view with the filtered workbench request list, along with the work status dropdown list.
        /// </returns>
        public async Task<IActionResult> _OfficerWorkBenchRequestsList(string fieldName, string searchText)
        {

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lists = await _IVendor.SearchVendorList(fieldName, searchText, loggedInUserId);

            var codeArray = new[] { "PND", "WORKING" };
            lists.WorkStatusDDL = _IDropdownList.GetDropdownListByArray(codeArray);

            return PartialView(lists);
        }

        /// <summary>
        /// Retrieves the delegation requests for the currently logged-in user and returns a partial view with the list.
        /// </summary>
        /// <param name="fieldName">The field name to search on (not used in this implementation).</param>
        /// <param name="searchText">The text to search for (not used in this implementation).</param>
        /// <returns>
        /// A partial view containing the list of delegation requests for the logged-in user.
        /// </returns>
        public async Task<IActionResult> _ManageDelegationRequestsList(string fieldName, string searchText)
        {
            var lists = await _iDelegationRequests.GetDelegationRequests(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return PartialView(lists);
        }

        /// <summary>
        /// Retrieves the vacation requests for the logged-in user, filtered by start and end dates, and returns a partial view with the list.
        /// </summary>
        /// <param name="filterStartDate">The start date for filtering vacation requests (defaults to 60 days before today's date).</param>
        /// <param name="filterEndDate">The end date for filtering vacation requests (defaults to today's date).</param>
        /// <returns>
        /// A partial view containing the filtered vacation requests for the logged-in user, along with relevant dropdown lists for reject reasons and work status.
        /// </returns>
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
            var workStatus = _IDropdownList.GetDropdownListByArray(codeArray);

            var model = new VendorViewModel
            {
                VacationList = lists,
                RejectReasonDDL = rejectReasonDDL,
                WorkStatusDDL = workStatus
            };
            return PartialView(model);
        }

        /// <summary>
        /// Updates the status of a vacation request and returns a JSON response indicating success or failure.
        /// </summary>
        /// <param name="model">The vacation request model containing the updated status information.</param>
        /// <returns>
        /// A JSON response containing a response code and message indicating whether the vacation request status was successfully updated or not.
        /// </returns>
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

        /// <summary>
        /// Retrieves and displays the vendor details based on the provided vendor initialization form ID.
        /// </summary>
        /// <param name="VendorsInitializationFormId">The unique identifier of the vendor initialization form.</param>
        /// <returns>
        /// A view containing the vendor details, including the department, position, and the name of the user who created the vendor record.
        /// </returns>
        public async Task<ViewResult> VendorViewTemplate(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            var user = await _IUser.GetUserByIdAsync(data.CreatedBy);
            var departmnet = await _IDropdownList.GetDropDownValueById(user.DepartmentId ?? Guid.Empty);

            data.Department = departmnet;
            data.Position = user.RoleName;
            data.CreatedBy = user.FullName;
            return View(data);
        }

        /// <summary>
        /// Retrieves and displays the vendor details for updating based on the provided vendor initialization form ID.
        /// This includes loading related information such as request type, user details, and department information.
        /// </summary>
        /// <param name="VendorsInitializationFormId">The unique identifier of the vendor initialization form.</param>
        /// <returns>
        /// A view containing the vendor details along with the necessary user and department data, 
        /// enabling the user to update the vendor details.
        /// </returns>
        public async Task<ViewResult> UpdateVendorDetails(Guid VendorsInitializationFormId)
        {
            var codeArray = new[] { "NEWVEN" };
            var RequestType = _IDropdownList.GetDropdownListByArray(codeArray);

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _IUser.GetUserByIdAsync(loggedInUserId);
            var department = await _IDropdownList.GetDropDownValueById(user.DepartmentId ?? Guid.Empty);

            var data = _IVendor.GetVendorById(VendorsInitializationFormId);

            //For Requested By
            var requestedByUser = await _IUser.GetUserByIdAsync(data.RequestedBy.ToString());
            data.RequestedByDepartment = await _IDropdownList.GetDropDownValueById(requestedByUser.DepartmentId ?? Guid.Empty);
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
                Department = department,
                Email = user.Email,
            };
            return View(model);
        }

        /// <summary>
        /// Retrieves the details of a user, including their department, position, and email, based on the provided user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// A JSON response containing the user's department, position, and email, or a response code of 0 if the user ID is invalid.
        /// </returns>
        public async Task<JsonResult> GetCreatedByInfo(Guid userId)
        {
            if (userId == Guid.Empty)
                return new JsonResult(new { responseCode = 0 });

            var user = await _IUser.GetUserByIdAsync(userId.ToString());
            var department = await _IDropdownList.GetDropDownValueById(user.DepartmentId ?? Guid.Empty);

            var data = new
            {
                Department = department,
                Position = user.RoleName,
                Email = user.Email
            };
            return new JsonResult(new { responseCode = 1, data = data });
        }

        /// <summary>
        /// Updates the vendor details based on the provided VendorDTO model.
        /// </summary>
        /// <param name="model">The VendorDTO model containing updated vendor information.</param>
        /// <returns>
        /// A JSON response with a response code and success message indicating the result of the update operation.
        /// </returns>
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
            var data = await _IVendor.UpdateVendorDetails(model, loggedInUserId);
            if (data)
            {
                return new JsonResult(new { responseCode = 0, SuccessResponse = "Successfully Update Record." });
            }
            return new JsonResult(new { responseCode = 1, SuccessResponse = "Please Try Again." });
        }

        /// <summary>
        /// Displays the details of a specific vacation request based on its ID.
        /// </summary>
        /// <param name="vacationRequestsId">The ID of the vacation request to view.</param>
        /// <returns>
        /// The view displaying the details of the vacation request.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> VacationView(Guid vacationRequestsId)
        {

            var data = await _iVacationRequests.GetVacationRequestsId(vacationRequestsId);
            return View(data);
        }

        /// <summary>
        /// Retrieves and displays a summary of the workbench, including counts of various work status for different approval types (VIR, VQR, User Approvals).
        /// </summary>
        /// <returns>
        /// A partial view containing the workbench summary with counts for each approval type based on the user's role.
        /// </returns>
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
            else if (userRole == "Vendor Officer")
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

        /// <summary>
        /// Deletes the selected vendors based on the provided list of GUIDs.
        /// </summary>
        /// <param name="selectedVendorGuids">A list of GUIDs representing the vendors to be deleted.</param>
        /// <returns>
        /// A JSON result indicating whether the deletion was successful or not.
        /// </returns>
        [HttpPost]
        public JsonResult deleteVendors([FromBody] List<Guid> selectedVendorGuids)
        {
            // Process the received GUIDs
            var isSuccess = _IVendor.DeleteVendors(selectedVendorGuids);
            // Return a success response
            return Json(new { success = isSuccess, message = "Data received successfully" });
        }

        /// <summary>
        /// Retrieves the details of a specific vendor based on the provided vendor ID and returns the corresponding view.
        /// </summary>
        /// <param name="VendorsInitializationFormId">The unique identifier for the vendor whose details are to be retrieved.</param>
        /// <returns>
        /// A view containing the details of the specified vendor.
        /// </returns>
        public ViewResult VendorDetails(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            return View(data);
        }

        /// <summary>
        /// Updates the vendor details based on the provided model.
        /// </summary>
        /// <param name="model">The data transfer object containing the updated vendor information.</param>
        /// <returns>
        /// A string indicating the result of the update operation, either a success message or "Failed" if an error occurred.
        /// </returns>
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

        /// <summary>
        /// Updates vacation requests based on the provided model.
        /// </summary>
        /// <param name="model">The data transfer object containing the updated vacation request information.</param>
        /// <returns>
        /// A string indicating the result of the update operation, either a success message or "Failed" if an error occurred.
        /// </returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vendorId"></param>
        /// <param name="isVendorCritical"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This method handles the creation of a delegate request. It checks if the logged-in user and delegate request data are valid, processes the request by updating the workflow statuses and adds the new delegate request.
        /// </summary>
        /// <param name="delegateRequestDTO">The data transfer object containing the delegate request details, including record IDs and record type.</param>
        /// <returns>Returns true if the delegate request is created successfully, otherwise false.</returns>
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

        /// <summary>
        /// This method handles the acceptance or rejection of a delegate request. It updates the status of the delegate request based on the provided input and logs the action for the logged-in user.
        /// </summary>
        /// <param name="delegateRequestId">The unique identifier of the delegate request.</param>
        /// <param name="isDelegationReqAccepted">A boolean indicating whether the delegate request is accepted (true) or rejected (false).</param>
        /// <returns>Returns true if the delegate request is successfully processed, otherwise false.</returns>
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
