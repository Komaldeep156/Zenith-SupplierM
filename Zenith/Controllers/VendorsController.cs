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

        /// <summary>
        /// Retrieves a list of vendors asynchronously and passes it to the Index view.
        /// </summary>
        /// <returns>Index view with vendor data</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _IVendor.GetVendors();
            return View(data);
        }

        /// <summary>
        /// Retrieves vendor details by ID and passes them to the VendorDetails view.
        /// </summary>
        /// <param name="VendorsInitializationFormId">The ID of the vendor initialization form.</param>
        /// <returns>VendorDetails view with the vendor data.</returns>
        public ViewResult VendorDetails(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            return View(data);
        }

        /// <summary>
        /// Retrieves vendor details by ID and populates additional information for the created by and requested by users.
        /// Passes the enriched data to the VendorViewTemplate view.
        /// </summary>
        /// <param name="VendorsInitializationFormId">The ID of the vendor initialization form.</param>
        /// <returns>VendorViewTemplate view with enriched vendor data.</returns>
        public async Task<ViewResult> VendorViewTemplate(Guid VendorsInitializationFormId)
        {
            var data = _IVendor.GetVendorById(VendorsInitializationFormId);
            var createdByUser = await _IUser.GetUserByIdAsync(data.CreatedBy);
            var requestedByUser = await _IUser.GetUserByIdAsync(data.RequestedBy.ToString());

            //For created By
            data.Department = await _IDropdownList.GetDropDownValuById(createdByUser.DepartmentId ?? Guid.Empty);
            data.Position = createdByUser.RoleName;
            data.CreatedBy = createdByUser.FullName;

            //For Requested By
            data.RequestedByDepartment = await _IDropdownList.GetDropDownValuById(requestedByUser.DepartmentId ?? Guid.Empty);
            data.RequestedByPosition = requestedByUser.RoleName;
            data.RequestedByName = requestedByUser.FullName;
            data.RequestedByEmail = requestedByUser.Email;
            return View(data);
        }

        /// <summary>
        /// Prepares the data required to add a new vendor and passes it to the AddVendor view.
        /// </summary>
        /// <returns>AddVendor view with a pre-populated VendorCreateModel.</returns>
        [HttpGet]
        public async Task<IActionResult> AddVendor()
        {
            var codeArray = new[] { "NEWVEN" };
            var RequestType = _IDropdownList.GetDropdownListByArry(codeArray);

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _IUser.GetUserByIdAsync(loggedInUserId);
            var departmnet = await _IDropdownList.GetDropDownValuById(user.DepartmentId ?? Guid.Empty);
            var model = new VendorCreateModel
            {
                UsersList = _IUser.GetUsers(),
                CreatedBy = user,
                RequestType = RequestType,
                Position = user.RoleName,
                Department = departmnet,
                Email = user.Email
            };
            return View(model);
        }

        /// <summary>
        /// Searches for vendors based on the specified field name and search text, 
        /// and returns a partial view with the filtered vendor list.
        /// </summary>
        /// <param name="fieldName">The field name to search by.</param>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>Partial view "_VendorListPartial" with the search results.</returns>
        public IActionResult SearchVendorList(string fieldName, string searchText)
        {
            var lists = _IVendor.SearchVendorList(fieldName, searchText);

            return PartialView("_VendorListPartial", lists);
        }

        /// <summary>
        /// Retrieves vendor details by the specified vendor initialization form ID.
        /// </summary>
        /// <param name="VendorsInitializationFormId">The ID of the vendor initialization form.</param>
        /// <returns>A DTO containing the vendor details.</returns>
        public GetVendorsListDTO GetVendorById(Guid VendorsInitializationFormId)
        {
            return _IVendor.GetVendorById(VendorsInitializationFormId);
        }

        /// <summary>
        /// Adds a new vendor using the provided model and the logged-in user's ID.
        /// </summary>
        /// <param name="model">The vendor data transfer object containing vendor details.</param>
        /// <returns>A JSON result indicating the outcome of the add operation.</returns>
        [HttpPost]
        public async Task<JsonResult> AddVendor(VendorDTO model)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Json(await _IVendor.AddVendor(model, loggedInUserId));
        }

        /// <summary>
        /// Adds a new address using the provided address model.
        /// </summary>
        /// <param name="model">The address data transfer object containing address details.</param>
        /// <returns>A JSON result indicating the outcome of the add operation.</returns>
        public JsonResult AddAddress(AddressDTO model)
        {
            return Json(_IVendor.AddAddress(model));
        }

        /// <summary>
        /// Adds a new vendor registration using the provided registration model.
        /// </summary>
        /// <param name="model">The registration data transfer object containing registration details.</param>
        /// <returns>A JSON result indicating the outcome of the registration process.</returns>
        public JsonResult AddNewRegistration(RegistrationDTO model)
        {
            return Json(_IVendor.AddNewRegistration(model));
        }

        /// <summary>
        /// Adds a new quality certification using the provided quality certification model.
        /// </summary>
        /// <param name="model">The quality certification data transfer object containing certification details.</param>
        /// <returns>A JSON result indicating the outcome of the add operation.</returns>
        public JsonResult AddQualityCertification(QualityCertificationDTO model)
        {
            return Json(_IVendor.AddQualityCertification(model));
        }

        /// <summary>
        /// Adds new payment terms using the provided payment terms model.
        /// </summary>
        /// <param name="model">The payment terms data transfer object containing payment terms details.</param>
        /// <returns>A JSON result indicating the outcome of the add operation.</returns>
        public JsonResult AddPaymentTerms(PaymentTermsDTO model)
        {
            return Json(_IVendor.AddPaymentTerms(model));
        }

        /// <summary>
        /// Adds new account details using the provided account details model.
        /// </summary>
        /// <param name="model">The account details data transfer object containing account information.</param>
        /// <returns>A JSON result indicating the outcome of the add operation.</returns>
        public JsonResult AddAccountDetails(AccountDetailsDTO model)
        {
            return Json(_IVendor.AddAccountDetails(model));
        }

        /// <summary>
        /// Adds other documents using the provided other documents model.
        /// </summary>
        /// <param name="model">The other documents data transfer object containing document details.</param>
        /// <returns>A JSON result indicating the outcome of the add operation.</returns>
        public JsonResult AddOtherDocuments([FromForm] OtherDocumentsDTO model)
        {
            return Json(_IVendor.AddOtherDocuments(model));
        }

        /// <summary>
        /// Downloads the Excel file containing the vendor initialization form template.
        /// </summary>
        /// <returns>A file result for downloading the Excel file or an error message if the file is not found or an exception occurs.</returns>
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

        /// <summary>
        /// Handles the upload of an Excel file to add new vendor records. It processes each row in the Excel file, 
        /// validates the data, and adds valid vendor records to the system. Invalid records are logged and returned.
        /// </summary>
        /// <param name="file">The Excel file containing the vendor data.</param>
        /// <returns>A JSON result indicating the number of successful and failed records, 
        /// and a download URL for the file containing invalid records.</returns>
        [HttpPost]
        public async Task<IActionResult> NewVendorUploadExcel(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<NewVendoFormDTO> notvalidRecords = new List<NewVendoFormDTO>();

            var vendorsDBList = await _IVendor.GetVendors();

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

        /// <summary>
        /// Downloads the Excel file containing invalid vendor records.
        /// </summary>
        /// <param name="filePath">The path to the Excel file containing the invalid records.</param>
        /// <returns>A file result for downloading the Excel file with invalid records.</returns>
        public IActionResult DownloadFailedVendorsFile(string filePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath); // Optional: Delete after downloading
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InvalidRecords.xlsx");
        }

        /// <summary>
        /// Generates an error message for invalid vendor record data, based on missing or incorrect values.
        /// </summary>
        /// <param name="requestedBy">The user requesting the vendor.</param>
        /// <param name="priorityId">The ID of the vendor's priority.</param>
        /// <param name="supplierTypeId">The ID of the vendor's supplier type.</param>
        /// <param name="contactCountryId">The ID of the vendor's contact country.</param>
        /// <returns>A string containing the error messages for the invalid fields.</returns>
        private string GetErrorMessage(object requestedBy, Guid priorityId, Guid supplierTypeId, Guid contactCountryId)
        {
            List<string> errors = new List<string>();

            if (requestedBy == null) errors.Add("Invalid RequestedBy Email");
            if (priorityId == Guid.Empty) errors.Add("Invalid Priority");
            if (supplierTypeId == Guid.Empty) errors.Add("Invalid SupplierType");
            if (contactCountryId == Guid.Empty) errors.Add("Invalid Country");

            return string.Join(", ", errors);
        }

        /// <summary>
        /// Generates an Excel file containing a list of invalid vendor records with error messages for each invalid record.
        /// </summary>
        /// <param name="notvalidRecords">The list of invalid vendor records with error messages.</param>
        /// <returns>A byte array representing the generated Excel file with invalid records and errors.</returns>
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
