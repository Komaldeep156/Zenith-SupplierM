using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

namespace Zenith.Controllers
{
    public class VendorsController : BaseController
    {
        private readonly IVendors _IVendor;
        private readonly IDropdownList _IDropdownList;
        private readonly IUser _IUser;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public VendorsController(IVendors IVendors,
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
        public ViewResult VendorDetails(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
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

        public GetVendorsListDTO GetVendorById(Guid VendorsInitializationFormId)
        {
            return _IVendor.GetVendorById(VendorsInitializationFormId);
        }

        [HttpPost]
        public JsonResult AddVendor(VendorDTO model)
        {

            return Json(_IVendor.AddVendor(model));
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

                    for (int row = 5; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var record = new NewVendoFormDTO
                        {
                            SNo = Convert.ToInt32(worksheet.Cells[row, 1].Text),
                            RequestType = worksheet.Cells[row, 2].Text,
                            RequestedByContactEmail = worksheet.Cells[row, 3].Text,
                            Priority = worksheet.Cells[row, 4].Text,
                            RequiredBy = worksheet.Cells[row, 5].Text,
                            SupplierName = worksheet.Cells[row, 6].Text,
                            SupplierType = worksheet.Cells[row, 7].Text,
                            Scope = worksheet.Cells[row, 8].Text,
                            ContactName = worksheet.Cells[row, 9].Text,
                            ContactPhone = worksheet.Cells[row, 10].Text,
                            Email = worksheet.Cells[row, 11].Text,
                            Country = worksheet.Cells[row, 12].Text,
                            BusinessCard = worksheet.Cells[row, 13].Text,
                            WebSite = worksheet.Cells[row, 14].Text
                        };

                        records.Add(record);
                    }
                }
            }

            List<NewVendoFormDTO> notvalidRecords = new List<NewVendoFormDTO>();
            var loginUser = _signInManager.IsSignedIn(User);
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (var item in records)
            {
                var requestedBy = _IUser.GetUserByEmail(item.RequestedByContactEmail);
                Guid PriorityId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.PRIORITY), item.Priority);
                Guid SupplierTypeId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.SUPPLIERTYPE), item.SupplierType);
                Guid ContactCountryId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.COUNTRY), item.Country);

                if (requestedBy == null || PriorityId==Guid.Empty || SupplierTypeId == Guid.Empty || ContactCountryId == Guid.Empty)
                {
                    notvalidRecords.Add(item);
                    continue;
                }

                VendorDTO vendor = new VendorDTO
                {
                    RequestedBy = new Guid(requestedBy.Id),
                    PriorityId = PriorityId,
                    RequiredBy = Convert.ToDateTime(item.RequiredBy),
                    SupplierName = item.SupplierName,
                    SupplierTypeId = SupplierTypeId,
                    Scope =item.Scope,
                    ContactName =item.ContactName,
                    ContactPhone =item.ContactPhone,
                    ContactEmail =item.Email,
                    ContactCountryId = ContactCountryId,
                    BusinessCard = item.BusinessCard,
                    Website = item.WebSite,
                    CreatedBy = new Guid(loggedInUserId),
                    StatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.VENDORSTATUS), nameof(DropDownValuesEnum.CREATED))
                };

                _IVendor.AddVendor(vendor);
            }
            return Ok();
        }
    }
}
