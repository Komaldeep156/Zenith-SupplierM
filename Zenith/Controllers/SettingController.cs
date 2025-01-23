using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    [Authorize]
    public class SettingController : BaseController
    {
        private readonly ISetting _ISetting;
        private readonly IVacationRequests _IVacationRequests;
        private readonly IUser _IUser;
        private readonly SignInManager<ApplicationUser> _SignInManager;
        public SettingController(IHttpContextAccessor httpContextAccessor, ISetting ISetting, IVacationRequests iVacationRequests, IUser Iuser,
            SignInManager<ApplicationUser> signInManager) : base(httpContextAccessor, signInManager)
        {
            _ISetting = ISetting;
            _SignInManager = signInManager;
            _IUser = Iuser;
            _IVacationRequests = iVacationRequests;
        }

        /// <summary>
        /// This method retrieves the details of the logged-in user by their unique ID and returns the view to display the user information.
        /// </summary>
        /// <returns>Returns the view displaying the details of the logged-in user.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await _IUser.GetUserByIdAsync(loggedInUserId);
            return View(data);
        }

        /// <summary>
        /// This method handles the process of changing the user's password. It validates the current password and updates it with the new password for the logged-in user.
        /// </summary>
        /// <param name="currentPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password to set for the user.</param>
        /// <returns>Returns a string indicating the result of the password change process.</returns>
        public async Task<string> ChangePassword(string currentPassword, string newPassword)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _ISetting.ChangePassword(currentPassword, newPassword, loggedInUserId);
        }

        /// <summary>
        /// This method handles the addition of vacation requests. It receives vacation request data and adds it for the logged-in user.
        /// </summary>
        /// <param name="vacationRequestsDTO">The data transfer object containing the details of the vacation request.</param>
        /// <returns>Returns an integer indicating the result of the vacation request addition process (e.g., success or failure status).</returns>
        public async Task<int> AddVacationRequests(VacationRequestsDTO vacationRequestsDTO)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _IVacationRequests.AddVacationRequests(vacationRequestsDTO, loggedInUserId);
        }

        /// <summary>
        /// This method retrieves a list of vacation requests for the logged-in user, filtered by a start and end date. If no dates are provided, it defaults to a 60-day range ending today. The result is returned as a partial view.
        /// </summary>
        /// <param name="filterStartDate">The start date for filtering vacation requests (optional). If not provided, defaults to 60 days before today.</param>
        /// <param name="filterEndDate">The end date for filtering vacation requests (optional). If not provided, defaults to today.</param>
        /// <returns>Returns a partial view displaying the list of vacation requests within the specified date range.</returns>
        public async Task<IActionResult> _VacationRequestsApprovalListPartialView(DateTime? filterStartDate = null, DateTime? filterEndDate = null)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DateTime todayDate = DateTime.Now;
            if (filterStartDate == null)
                filterStartDate = todayDate.AddDays(-60);
            if (filterEndDate == null)
                filterEndDate = todayDate;
            var lists = await _IVacationRequests.GetAccountVacationRequests(loggedInUserId, Convert.ToDateTime(filterStartDate), Convert.ToDateTime(filterEndDate));
            return PartialView(lists);
        }
    }
}
