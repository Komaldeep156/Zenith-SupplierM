using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.Controllers
{
    public class VendorsController : BaseController
    {
        private readonly IVendors _IVendor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VendorsController(IVendors IVendors,
                                IHttpContextAccessor httpContextAccessor,
                                SignInManager<ApplicationUser> signInManager,
                                IWebHostEnvironment webHostEnvironment)
      : base(httpContextAccessor, signInManager)
        {
            _IVendor = IVendors;
            _webHostEnvironment = webHostEnvironment;
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

        public IActionResult DownloadExcelFile()
        {
            try
            {
                // Path to the folder where the Excel files are stored
                string rootPath = _webHostEnvironment.ContentRootPath;
                string folderPath = Path.Combine(rootPath, "ExcelTemplates");
                string filePath = Path.Combine(folderPath, "VendorInitializationformTemplate.xlsx");

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                // Read file bytes
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                // Return file for download
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "VendorInitializationformTemplate.xlsx");
            }
            catch (Exception ex)
            {
                // Log or handle exception as needed
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult NewVendorUploadExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            var records = new List<NewVendoFormDTO>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    for (int row = 3; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var record = new NewVendoFormDTO
                        {
                            SNo = Convert.ToInt32(worksheet.Cells[row, 1].Text),
                            RequestType = worksheet.Cells[row, 2].Text,
                            RequestedByContactName = worksheet.Cells[row, 3].Text,
                            Priority = worksheet.Cells[row, 4].Text,
                            SupplierName = worksheet.Cells[row, 5].Text,
                            SupplierType = worksheet.Cells[row, 6].Text,
                            Scope = worksheet.Cells[row, 7].Text,
                            ContactName = worksheet.Cells[row, 8].Text,
                            ContactPhone = worksheet.Cells[row, 9].Text,
                            Email = worksheet.Cells[row, 10].Text,
                            Country = worksheet.Cells[row, 11].Text,
                            BusinessCard = worksheet.Cells[row, 12].Text,
                            WebSite = worksheet.Cells[row, 13].Text
                        };

                        records.Add(record);
                    }
                }
            }

            return Ok(records);
        }
    }
}
