using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUser _IUser;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUser IUser, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) : base(httpContextAccessor, signInManager)
        {
            _userManager = userManager;
            _IUser = IUser;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var data = _IUser.GetUsers();
            return View(data);
        }

        [HttpPost]
        public Task<string> AddNewUser(RegisterUserModel model)
        {
            Guid tenantId = Guid.Parse(HttpContext.Session.GetString("tenantId"));

            var requestScheme = Request.Scheme;
            return _IUser.AddNewUser(model, Url, requestScheme, tenantId);
        }

        [HttpPost]
        public Task<string> UpdateUser(RegisterUserModel model)
        {
            try
            {
                return _IUser.UpdateUser(model);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet]
        public GetUserListDTO GetUserById(string userId)
        {
            try
            {
                return _IUser.GetUserById(userId);
            }
            catch (Exception ex)
            {
                throw ex;
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
