using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    public class VendorsController : BaseController
    {
        private readonly IVendors _IVendor;
        public VendorsController(IVendors IVendors,
                                IHttpContextAccessor httpContextAccessor,
                                SignInManager<ApplicationUser> signInManager)
      : base(httpContextAccessor, signInManager)
        {
            _IVendor = IVendors;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var data = _IVendor.GetVendors();
            return View(data);
        }
        public ViewResult VendorDetails(Guid vendorId)
        {
            var data = _IVendor.GetVendorById(vendorId);
            return View(data);
        }

        [HttpGet]
        public IActionResult AddVendor()
        {
            return View();
        }

        public List<GetVendorsListDTO> SearchVendorList(string fieldName, string searchText)
        {
            return _IVendor.SearchVendorList(fieldName, searchText);
        }

        public GetVendorsListDTO GetVendorById(Guid vendorId)
        {
            return _IVendor.GetVendorById(vendorId);
        }

        [HttpPost]
        public JsonResult AddVendor(VendorDTO model)
        {
            Guid tenantId = Guid.Parse(HttpContext.Session.GetString("tenantId"));

            return Json(_IVendor.AddVendor(model, tenantId));
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
        public JsonResult AddAddress(AddressDTO model)
        {
            return Json(_IVendor.AddAddress(model));
        }
        public JsonResult AddNewRegistration(RegistrationDTO model)
        {
            return Json(_IVendor.AddNewRegistration(model));
        }
        public JsonResult AddQualityCertification(QualityCertificationDTO model)
        {
            return Json(_IVendor.AddQualityCertification(model));
        }
        public JsonResult AddPaymentTerms(PaymentTermsDTO model)
        {
            return Json(_IVendor.AddPaymentTerms(model));
        }
        public JsonResult AddAccountDetails(AccountDetailsDTO model)
        {
            return Json(_IVendor.AddAccountDetails(model));
        }
        public JsonResult AddOtherDocuments([FromForm] OtherDocumentsDTO model)
        {
            return Json(_IVendor.AddOtherDocuments(model));
        }
    }
}
