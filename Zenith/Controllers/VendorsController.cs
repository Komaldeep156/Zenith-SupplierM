using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Security.Claims;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;

namespace Zenith.Controllers
{
    [Authorize]
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

        public ViewResult VendorViewTemplate(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> AddVendor()
        {
            var RequestType = new List<string>();
            RequestType.Add("New Vendor");

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _IUser.GetUserByIdAsync(loggedInUserId);
            var departmnet = await _IDropdownList.GetDropDownValuById(user.DepartmentId ?? Guid.Empty);
            var model = new VendorCreateModel
            {
                CreatedBy = user.FullName,
                UsersList = _IUser.GetUsers(),
                RequestType = RequestType,
                Position = user.RoleName,
                Department = departmnet,
                Email = user.Email
            };
            return View(model);
        }

        public IActionResult SearchVendorList(string fieldName, string searchText)
        {
            var lists = _IVendor.SearchVendorList(fieldName, searchText);

            return PartialView("_VendorListPartial", lists);
        }

        public GetVendorsListDTO GetVendorById(Guid VendorsInitializationFormId)
        {
            return _IVendor.GetVendorById(VendorsInitializationFormId);
        }

        [HttpPost]
        public async Task<JsonResult> AddVendor(VendorDTO model)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Json(await _IVendor.AddVendor(model, loggedInUserId));
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
            List<NewVendoFormDTO> notvalidRecords = new List<NewVendoFormDTO>();

            var vendorsDBList = _IVendor.GetVendors();

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
                            SupplierCountry = worksheet.Cells[row, 8].Text,
                            Scope = worksheet.Cells[row, 9].Text,
                            ContactName = worksheet.Cells[row, 10].Text,
                            ContactPhone = worksheet.Cells[row, 11].Text,
                            Email = worksheet.Cells[row, 12].Text,
                            Country = worksheet.Cells[row, 13].Text,
                            WebSite = worksheet.Cells[row, 14].Text
                        };

                        records.Add(record);
                    }
                }
            }

            var loginUser = _signInManager.IsSignedIn(User);
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int successCount = 0;
            int failureCount = 0;

            foreach (var item in records)
            {
                var requestedBy = _IUser.GetUserByEmail(item.RequestedByContactEmail);
                Guid PriorityId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.PRIORITY), item.Priority);
                Guid SupplierTypeId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.SUPPLIERTYPE), item.SupplierType);
                Guid ContactCountryId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.COUNTRY), item.Country);
                Guid SupplierCountryId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.COUNTRY), item.SupplierCountry);

                requestedBy = null;
                ContactCountryId = Guid.Empty;
                if (requestedBy == null || PriorityId == Guid.Empty || SupplierTypeId == Guid.Empty || ContactCountryId == Guid.Empty || SupplierCountryId == Guid.Empty)
                {
                    item.ErrorMessage = GetErrorMessage(requestedBy, PriorityId, SupplierTypeId, ContactCountryId);
                    notvalidRecords.Add(item);
                    failureCount++;
                    continue;
                }

                if ((vendorsDBList.Vendors.Where(x => x.SupplierName.Trim() == item.SupplierName.Trim()
                        && x.SupplierCountryId == item.SupplierCountryId).Any()) ||
                        records.Where(x => x.SupplierName.Trim() == item.SupplierName.Trim()
                        && x.SupplierCountryId == item.SupplierCountryId).Count() > 1)
                {
                    item.ErrorMessage = "Duplicate record found in the system";
                    notvalidRecords.Add(item);
                    failureCount++;
                    continue;
                }

                VendorDTO vendor = new VendorDTO
                {
                    RequestedBy = new Guid(requestedBy.Id),
                    PriorityId = PriorityId,
                    RequiredBy = Convert.ToDateTime(item.RequiredBy),
                    SupplierName = item.SupplierName,
                    SupplierTypeId = SupplierTypeId,
                    Scope = item.Scope,
                    ContactName = item.ContactName,
                    ContactPhone = item.ContactPhone,
                    ContactEmail = item.Email,
                    ContactCountryId = ContactCountryId,
                    Website = item.WebSite,
                    CreatedBy = loggedInUserId,
                    StatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.CREATED))
                };

                _IVendor.AddVendor(vendor, loggedInUserId);
                successCount++;
            }

            // Check if there are invalid records
            string fileUrl = null;
            if (notvalidRecords.Any())
            {
                var excelFile = GenerateInvalidRecordsExcel(notvalidRecords);
                string filePath = Path.Combine(Path.GetTempPath(), "InvalidRecords.xlsx");
                System.IO.File.WriteAllBytes(filePath, excelFile); // Save to temporary location
                fileUrl = Url.Action("DownloadFailedVendorsFile", new { filePath }); // Generate download URL
            }

            return Json(new
            {
                successCount,
                failureCount,
                fileUrl
            });

        }

        // Separate action for downloading the Excel file
        public IActionResult DownloadFailedVendorsFile(string filePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath); // Optional: Delete after downloading
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InvalidRecords.xlsx");
        }

        private string GetErrorMessage(object requestedBy, Guid priorityId, Guid supplierTypeId, Guid contactCountryId)
        {
            List<string> errors = new List<string>();

            if (requestedBy == null) errors.Add("Invalid RequestedBy Email");
            if (priorityId == Guid.Empty) errors.Add("Invalid Priority");
            if (supplierTypeId == Guid.Empty) errors.Add("Invalid SupplierType");
            if (contactCountryId == Guid.Empty) errors.Add("Invalid Country");

            return string.Join(", ", errors);
        }

        private byte[] GenerateInvalidRecordsExcel(List<NewVendoFormDTO> notvalidRecords)
        {
            using (var package = new ExcelPackage())
            {
                // Add a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Invalid Records");

                // Define headers
                var headers = new[] { "Serial Number", "Vendor Name", "Requested By", "Priority", "Supplier Type", "Country", "Error" };

                // Add headers to the worksheet with formatting
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cells[1, i + 1];
                    cell.Value = headers[i];

                    // Set header background color and style
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray); // Customize header color
                    cell.Style.Font.Bold = true;
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Add data and style "Error" column in red
                int row = 2;
                foreach (var record in notvalidRecords)
                {
                    worksheet.Cells[row, 1].Value = row - 1; // Serial Number
                    worksheet.Cells[row, 2].Value = record.SupplierName;
                    worksheet.Cells[row, 3].Value = record.RequestedByContactEmail;
                    worksheet.Cells[row, 4].Value = record.Priority;
                    worksheet.Cells[row, 5].Value = record.SupplierType;
                    worksheet.Cells[row, 6].Value = record.Country;

                    // Set the "Error" message and style it in red
                    var errorCell = worksheet.Cells[row, 7];
                    errorCell.Value = record.ErrorMessage; // Customize error message
                    errorCell.Style.Font.Color.SetColor(Color.Red);

                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return Excel file as byte array
                return package.GetAsByteArray();
            }
        }
    }
}
