using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

namespace Zenith.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = await _IUser.GetUserByIdAsync(loggedInUserId);
            return View(data);
        } 
        
        public async Task<string> ChangePassword(string currentPassword, string newPassword)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _ISetting.ChangePassword(currentPassword, newPassword, loggedInUserId);
        }

        public async Task<int> AddVacationRequests(VacationRequestsDTO vacationRequestsDTO)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _IVacationRequests.AddVacationRequests(vacationRequestsDTO, loggedInUserId);
        }

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
