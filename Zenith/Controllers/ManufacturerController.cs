using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    public class ManufacturerController : BaseController
    {
        private readonly IManufacturer _IManufacturer;

        public ManufacturerController(IManufacturer IManufacturer,
                                  IHttpContextAccessor httpContextAccessor,
                                  SignInManager<ApplicationUser> signInManager)
        : base(httpContextAccessor, signInManager)
        {
            _IManufacturer = IManufacturer;
        }
        [HttpGet]
        public ActionResult Index()
        {
            var data = _IManufacturer.getManufacture();
            return View(data);
        }

        [HttpPost]
        public JsonResult AddManufacturer(ManufacturerDTO model)
        {
            Guid tenantId = Guid.Parse(HttpContext.Session.GetString("tenantId"));

            return Json(_IManufacturer.AddManufacturer(model, tenantId));
        }

        [HttpGet]
        public GetManufactureListDTO GetManufacturerById(Guid ManufacturerId)
        {
            try
            {
                return _IManufacturer.GetManufacturerById(ManufacturerId);
            }catch (Exception ex)
            {
                throw new Exception (ex.Message);
            }
        }
        public JsonResult AddNewBrand([FromForm] BrandDTO model)
        {
            return Json(_IManufacturer.AddNewBrand(model));
        }
        public JsonResult AddNewProduct(ProductDTO model)
        {
            return Json(_IManufacturer.AddNewProduct(model));
        }


    }
}
