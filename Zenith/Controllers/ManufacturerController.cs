using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    [Authorize]
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

        /// <summary>
        /// This method retrieves the list of manufacturers and returns the view to display them.
        /// </summary>
        /// <returns>Returns the view displaying the list of manufacturers.</returns>
        [HttpGet]
        public ActionResult Index()
        {
            var data = _IManufacturer.getManufacture();
            return View(data);
        }

        /// <summary>
        /// This method handles the addition of a new manufacturer. It receives the manufacturer data and returns a JSON response indicating the result of the addition process.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the manufacturer to be added.</param>
        /// <returns>Returns a JSON response indicating the success or failure of the manufacturer addition.</returns>
        [HttpPost]
        public JsonResult AddManufacturer(ManufacturerDTO model)
        {
            return Json(_IManufacturer.AddManufacturer(model));
        }

        /// <summary>
        /// This method retrieves the details of a manufacturer by its unique ID.
        /// </summary>
        /// <param name="ManufacturerId">The unique identifier of the manufacturer.</param>
        /// <returns>Returns the manufacturer details as a data transfer object.</returns>
        [HttpGet]
        public GetManufactureListDTO GetManufacturerById(Guid ManufacturerId)
        {
            try
            {
                return _IManufacturer.GetManufacturerById(ManufacturerId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// This method handles the addition of a new brand. It receives brand data from the form and returns a JSON response indicating the result of the addition process.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the brand to be added.</param>
        /// <returns>Returns a JSON response indicating the success or failure of the brand addition.</returns>
        public JsonResult AddNewBrand([FromForm] BrandDTO model)
        {
            return Json(_IManufacturer.AddNewBrand(model));
        }

        /// <summary>
        /// This method handles the addition of a new product. It receives product data and returns a JSON response indicating the result of the product addition process.
        /// </summary>
        /// <param name="model">The data transfer object containing the details of the product to be added.</param>
        /// <returns>Returns a JSON response indicating the success or failure of the product addition.</returns>
        public JsonResult AddNewProduct(ProductDTO model)
        {
            return Json(_IManufacturer.AddNewProduct(model));
        }
    }
}
