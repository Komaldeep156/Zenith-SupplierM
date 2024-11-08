using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

namespace Zenith.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUser _IUser;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDropdownList _IDropdownList;

        public UserController(IUser IUser, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, IDropdownList iDropdownList) : base(httpContextAccessor, signInManager)
        {
            _userManager = userManager;
            _IUser = IUser;
            _IDropdownList = iDropdownList;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var data = _IUser.GetUsers();
            return View(data);
        }

        [HttpGet]
        public IActionResult UserViewTemplate(string userId)
        {
            try
            {
                /*var reportingManagerDDL = _IDropdownList.GetDropdownByName(RolesEnum.REPORTING_MANAGER.ToString().Replace("_", " ").ToUpper());
                ViewBag.reportingManagers = reportingManagerDDL;*/
                var data = _IUser.GetUserById(userId);
                return View(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public List<GetUserListDTO> GetUsers()
        {
            return _IUser.GetUsers();
        }
        
        [HttpGet]
        public async Task<List<ApplicationUser>> GetReportingManagersAsync()
        {
            return await _IUser.GetReportingManagersAsync();
        }

        [HttpPost]
        public async Task<string> AddNewUser(RegisterUserModel model)
        {
            try
            {
                var requestScheme = Request.Scheme;
                return await _IUser.AddNewUser(model, Url, requestScheme);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<string> UpdateUser(RegisterUserModel model)
        {
            try
            {
                return await _IUser.UpdateUser(model);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<bool> UpdateUserActiveInactive(string userId, bool isActive)
        {
            try
            {
                return await _IUser.UpdateUserActiveInactive(userId, isActive);
            }
            catch (Exception)
            {
                return false;
            }
        }

       public JsonResult AddContact(ContactDTO model)
        {
            return Json(_IUser.AddContact(model));
        }
        public JsonResult AddFile(AttachmentDTO File)
        {
            return Json(_IUser.AddFile(File));
        }
    }
}
