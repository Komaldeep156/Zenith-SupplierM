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
        private readonly IUser _IUser;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public WorkbenchController(IVendors IVendors,
                                IHttpContextAccessor httpContextAccessor,
                                SignInManager<ApplicationUser> signInManager,
                                IWebHostEnvironment webHostEnvironment,
                                IUser iUser, IDropdownList iDropdownList)
      : base(httpContextAccessor, signInManager)
        {
            _IVendor = IVendors;
            _webHostEnvironment = webHostEnvironment;
            _IUser = iUser;
            _IDropdownList = iDropdownList;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var data = _IVendor.GetVendors();
            return View(data);
        }

        public IActionResult _VendorApprovalListPartialView(string fieldName, string searchText)
        {
            var lists = _IVendor.SearchVendorList(fieldName, searchText);

            return PartialView(lists);
        }

        public ViewResult VendorViewTemplate(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            return View(data);
        }

        public ViewResult VendorDetails(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            return View(data);
        }

        [HttpPost]
        public Task<string> UpdateVendor(updateVendorDTO model)
        {
            try
            {
                return _IVendor.UpdateVendor(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
