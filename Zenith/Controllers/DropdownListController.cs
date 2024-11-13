using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    public class DropdownListController : BaseController
    {
        private readonly IDropdownList _dropdownList;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public DropdownListController( IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            IDropdownList dropdownList) : base(httpContextAccessor, signInManager)
        {
            _signInManager = signInManager;
            _dropdownList = dropdownList;
        }

        public ActionResult Index()
        {
            var data = _dropdownList.GetDropdownList();
            return View(data);
        }

        [HttpGet]
        public GetDropdownListDTO GetDropdownByName(string name)
        {
            try
            {
                return _dropdownList.GetDropdownByName(name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public Task<string> AddNewList(DropdownLists model)
        {
            try
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return _dropdownList.AddNewList(model, loggedInUserId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
           
        }

        [HttpPost]
        public Task<string> AddValue([FromBody] DropdownValueDTO model)
        {
            try
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return _dropdownList.AddValue(model, loggedInUserId);
            }catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
