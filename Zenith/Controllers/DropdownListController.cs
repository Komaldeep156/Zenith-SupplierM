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
        public DropdownListController(IHttpContextAccessor httpContextAccessor,
            SignInManager<ApplicationUser> signInManager,
            IDropdownList dropdownList) : base(httpContextAccessor, signInManager)
        {
            _signInManager = signInManager;
            _dropdownList = dropdownList;
        }

        /// <summary>
        /// This method retrieves the data for the dropdown list and returns the view to display the list.
        /// </summary>
        /// <returns>Returns the view displaying the dropdown list data.</returns>
        public ActionResult Index()
        {
            var data = _dropdownList.GetDropdownList();
            return View(data);
        }

        /// <summary>
        /// This method retrieves a dropdown list based on the specified name.
        /// It handles any exceptions that occur during the retrieval process and throws an exception with the relevant message if an error occurs.
        /// </summary>
        /// <param name="name">The name of the dropdown list to retrieve.</param>
        /// <returns>Returns the dropdown list data transfer object for the specified name.</returns>
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

        /// <summary>
        /// This method handles the addition of a new dropdown list. 
        /// It takes the list data, adds it to the system, and returns a string response indicating the result of the addition process.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the dropdown list to be added.</param>
        /// <returns>Returns a string response indicating the success or failure of the dropdown list addition process.</returns>
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

        /// <summary>
        /// This method handles the addition of a new value to a dropdown list. 
        /// It receives the value data, adds it to the system, and returns a string response indicating the result of the addition process.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the dropdown value to be added.</param>
        /// <returns>Returns a string response indicating the success or failure of the dropdown value addition process.</returns>
        [HttpPost]
        public Task<string> AddValue([FromBody] DropdownValueDTO model)
        {
            try
            {
                var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return _dropdownList.AddValue(model, loggedInUserId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
