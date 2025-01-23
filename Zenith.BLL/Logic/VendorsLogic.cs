using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
using Zenith.Repository.RepositoryFiles;
using static Zenith.BLL.DTO.GetVendorsListDTO;

namespace Zenith.BLL.Logic
{
    public class VendorsLogic : IVendors
    {
        private readonly IRepository<VendorsInitializationForm> _vendorRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Registrations> _registrationRepository;
        private readonly IRepository<QualityCertification> _qualityCertificationRepository;
        private readonly IRepository<AccountDetails> _accountDetailRepository;
        private readonly IRepository<OtherDocuments> _otherRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDropdownList _IDropdownList;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<VendorQualificationWorkFlow> _VendorQualificationWorkFlowrepo;
        private readonly IVendorQualificationWorkFlowExecution _vendorQualificationWorkFlowExecution;
        private readonly IWorkFlows _workflows;
        public readonly ZenithDbContext _zenithDbContext;
        public readonly EmailUtils _emailUtils;

        public VendorsLogic(IRepository<VendorsInitializationForm> vendorRepository, IRepository<Address> AddressRepository,
            IRepository<Registrations> RegistrationRepository, IRepository<QualityCertification> QualityCertificationRepository,
            IRepository<AccountDetails> accountDetailRepository, IRepository<OtherDocuments> otherRepository,
            RoleManager<IdentityRole> roleManager, IDropdownList iDropdownList, UserManager<ApplicationUser> userManager,
            IRepository<VendorQualificationWorkFlow> vendorQualificationWorkFlowrepo,
            IVendorQualificationWorkFlowExecution vendorQualificationWorkFlowExecution,
            ZenithDbContext zenithDbContext, IWorkFlows workflows, EmailUtils emailUtils)
        {
            _vendorRepository = vendorRepository;
            _addressRepository = AddressRepository;
            _registrationRepository = RegistrationRepository;
            _qualityCertificationRepository = QualityCertificationRepository;
            _accountDetailRepository = accountDetailRepository;
            _otherRepository = otherRepository;
            _roleManager = roleManager;
            _IDropdownList = iDropdownList;
            _userManager = userManager;
            _VendorQualificationWorkFlowrepo = vendorQualificationWorkFlowrepo;
            _vendorQualificationWorkFlowExecution = vendorQualificationWorkFlowExecution;
            _zenithDbContext = zenithDbContext;
            _workflows = workflows;
            _emailUtils = emailUtils;
        }

        /// <summary>
        /// Retrieves the value of a column from a data reader or returns the default value if the column is null.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="reader">The data reader.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The value of the column or the default value if the column is null.</returns>
        private T GetValueOrDefault<T>(IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return default;

            object value = reader.GetValue(ordinal);

            // Handle special type conversions
            if (typeof(T) == typeof(bool) && value is int intValue)
            {
                return (T)(object)(intValue != 0); // Convert int to bool
            }

            return (T)value; // Default casting
        }

        /// <summary>
        /// Retrieves a list of vendors based on the provided filters.
        /// </summary>
        /// <param name="assignUserId">The ID of the assigned user.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="searchText">The search text to filter vendors.</param>
        /// <returns>A list of vendor DTOs.</returns>
        public async Task<List<GetVendorsListDTO>> GetVendorsBySpa(string assignUserId = null, string fieldName = null, string searchText = null)
        {
            var vendorList = new List<GetVendorsListDTO>();

            try
            {
                var connectionString = _zenithDbContext.Database.GetConnectionString();
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand("GETVENDORDETAILS", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters if they are provided
                if (!string.IsNullOrEmpty(assignUserId))
                {
                    command.Parameters.AddWithValue("@AssignUserId", assignUserId);
                }
                if (!string.IsNullOrEmpty(fieldName))
                {
                    command.Parameters.AddWithValue("@fieldName", fieldName);
                }
                if (!string.IsNullOrEmpty(searchText))
                {
                    command.Parameters.AddWithValue("@SearchText", searchText);
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    vendorList.Add(new GetVendorsListDTO
                    {
                        Id = GetValueOrDefault<Guid>(reader, "Id"),
                        PriorityType = GetValueOrDefault<string>(reader, "PriorityType"),
                        SupplierName = GetValueOrDefault<string>(reader, "SupplierName"),
                        RequiredBy = GetValueOrDefault<DateTime>(reader, "RequiredBy"),
                        SupplierType = GetValueOrDefault<string>(reader, "SupplierType"),
                        SupplierTypeId = GetValueOrDefault<Guid>(reader, "SupplierTypeId"),
                        Scope = GetValueOrDefault<string>(reader, "Scope"),
                        ContactName = GetValueOrDefault<string>(reader, "ContactName"),
                        ContactPhone = GetValueOrDefault<string>(reader, "ContactPhone"),
                        ContactEmail = GetValueOrDefault<string>(reader, "ContactEmail"),
                        ContactCountryId = GetValueOrDefault<Guid>(reader, "ContactCountryId"),
                        ContactCountry = GetValueOrDefault<string>(reader, "ContactCountry"),
                        Website = GetValueOrDefault<string>(reader, "Website"),
                        RequestNum = GetValueOrDefault<string>(reader, "RequestNum"),
                        StatusId = GetValueOrDefault<Guid>(reader, "StatusId"),
                        IsCritical = GetValueOrDefault<bool>(reader, "IsCritical"),
                        IsApproved = GetValueOrDefault<bool>(reader, "IsApproved"),
                        RejectionReason = GetValueOrDefault<string>(reader, "RejectionReason"),
                        Comments = GetValueOrDefault<string>(reader, "Comments"),
                        IsActive = GetValueOrDefault<bool>(reader, "IsActive"),
                        SupplierCountryId = GetValueOrDefault<Guid>(reader, "SupplierCountryId"),
                        SupplierCountry = GetValueOrDefault<string>(reader, "SupplierCountry"),
                        CreatedBy = GetValueOrDefault<string>(reader, "CreatedBy"),
                        CreatedByName = GetValueOrDefault<string>(reader, "FullName"),
                        CreatedOn = GetValueOrDefault<DateTime>(reader, "CreatedOn"),
                        ModifiedOn = GetValueOrDefault<DateTime>(reader, "ModifiedOn"),
                        ModifiedBy = GetValueOrDefault<string>(reader, "ModifiedBy"),
                        IsDeleted = GetValueOrDefault<bool>(reader, "IsDeleted"),
                        RequestStatusDescription = GetValueOrDefault<string>(reader, "RequestStatusDescription"),
                        IsDelgateRequested = GetValueOrDefault<bool>(reader, "IsDelegateRequested"),
                        DueDays = GetValueOrDefault<int>(reader, "DueDays"),
                        WorkStatus = GetValueOrDefault<string>(reader, "WorkStatus"),
                        WorkStatusId = GetValueOrDefault<Guid>(reader, "WorkStatusId"),
                        RequestStatus = GetValueOrDefault<string>(reader, "RequestStatus"),
                        AssignUser = GetValueOrDefault<string>(reader, "AssignUser"),
                        RequestStatusCode = GetValueOrDefault<string>(reader, "RequestStatusCode"),
                        WorkStatusCode = GetValueOrDefault<string>(reader, "WorkStatusCode"),
                        FirstApproverName = GetValueOrDefault<string>(reader, "FirstApproverName")
                    });
                }
            }
            catch (Exception ex)
            {
                var error = ex.InnerException;
            }

            return vendorList;
        }

        /// <summary>
        /// Retrieves a list of vendors.
        /// </summary>
        /// <param name="assignUserId">The ID of the assigned user.</param>
        /// <returns>A vendor view model containing a list of vendors.</returns>
        public async Task<VendorViewModel> GetVendors(string assignUserId = default)
        {
            var vendorList = await GetVendorsBySpa(assignUserId);

            var model = new VendorViewModel
            {
                Vendors = vendorList.Any() ? vendorList : new List<GetVendorsListDTO>()
            };

            return model;
        }

        /// <summary>
        /// Deletes multiple vendors.
        /// </summary>
        /// <param name="selectedVendorIds">A list of vendor IDs to be deleted.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public bool DeleteVendors(List<Guid> selectedVendorIds)
        {
            if (selectedVendorIds != null)
            {
                foreach (var vendor in selectedVendorIds)
                {

                    var dbVendor = _vendorRepository.Where(x => x.Id == vendor).FirstOrDefault();
                    if (dbVendor != null)
                        _vendorRepository.Remove(dbVendor);
                    _vendorRepository.SaveChanges();
                }
            }
            return true;
        }

        /// <summary>
        /// Retrieves a vendor by its ID.
        /// </summary>
        /// <param name="VendorsInitializationFormId">The ID of the vendor.</param>
        /// <returns>A vendor DTO.</returns>
        public GetVendorsListDTO GetVendorById(Guid VendorsInitializationFormId)
        {
            var vendor = (from a in _vendorRepository
                          where a.Id == VendorsInitializationFormId
                          select new GetVendorsListDTO
                          {
                              Id = a.Id,
                              SupplierName = a.SupplierName,
                              RequestNum = a.RequestNum,
                              DropdownValues_Priority = a.DropdownValues_Priority,
                              RequiredBy = a.RequiredBy,
                              RequestedBy = a.RequestedBy,
                              CreatedBy = a.CreatedBy,
                              DropdownValues_SupplierType = a.DropdownValues_SupplierType,
                              Scope = a.Scope,
                              ContactName = a.ContactName,
                              ContactPhone = a.ContactPhone,
                              ContactEmail = a.ContactEmail,
                              DropdownValues_ContactCountry = a.DropdownValues_ContactCountry,
                              Website = a.Website,
                              DropdownValues_Status = a.DropdownValues_Status,
                              IsCritical = a.IsCritical,
                              IsApproved = a.IsApproved,
                              DropdownValues_RejectionReason = a.DropdownValues_RejectionReason,
                              Comments = a.Comments,
                              DropdownValues_SupplierCountry = a.DropdownValues_SupplierCountry,
                              IsActive = a.IsActive,
                              ApplicationUser_CreatedBy = a.ApplicationUser_CreatedBy,
                              CreatedOn = a.CreatedOn,
                              ModifiedBy = a.ModifiedBy,
                              ModifiedOn = a.ModifiedOn,
                              BusinessRegistrationNo = a.BusinessRegistrationNo
                          }).FirstOrDefault();

            return vendor;
        }

        /// <summary>
        /// Searches for vendors based on the provided filters.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="searchText">The search text to filter vendors.</param>
        /// <param name="assignUserId">The ID of the assigned user.</param>
        /// <returns>A vendor view model containing a list of vendors.</returns>
        public async Task<VendorViewModel> SearchVendorList(string fieldName, string searchText, string assignUserId = default)
        {
            var vendorList = await GetVendorsBySpa(assignUserId, fieldName, searchText);
            var model = new VendorViewModel
            {
                Vendors = vendorList.Any() ? vendorList : new List<GetVendorsListDTO>()
            };

            return model;
        }

        /// <summary>
        /// Adds a new vendor.
        /// </summary>
        /// <param name="model">The vendor DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        public async Task<int> AddVendor(VendorDTO model, string loggedInUserId)
        {
            var virRejectstatusId = (await _zenithDbContext.DropdownValues.FirstOrDefaultAsync(x => x.Code == "VIRRJCTD"))?.Id ?? Guid.Empty;
            var virCancelledstatusId = (await _zenithDbContext.DropdownValues.FirstOrDefaultAsync(x => x.Code == "VIRCANDTDUPLI"))?.Id ?? Guid.Empty;
            if (_vendorRepository.Where(x => x.BusinessRegistrationNo == model.BusinessRegistrationNo
                                && x.SupplierCountryId == model.SupplierCountryId
                                && x.SupplierName == model.SupplierName
                                && (x.IsActive || (x.StatusId != virRejectstatusId && x.StatusId != virCancelledstatusId))).Any())
            {
                return 2;
            }
            if (!model.allowDuplicateVendor && (_vendorRepository.Where(x => x.SupplierName.Trim() == model.SupplierName.Trim()
                       && x.SupplierCountryId == model.SupplierCountryId).Any()))
            {
                return 0;
            }

            string uniqueCode = GenerateUniqueCode();
            string ShortName = GenerateShortName(model.SupplierName);
            VendorsInitializationForm obj = new VendorsInitializationForm
            {
                RequestedBy = model.RequestedBy,
                PriorityId = model.PriorityId,
                RequiredBy = model.RequiredBy,
                SupplierName = model.SupplierName,
                SupplierTypeId = model.SupplierTypeId,
                SupplierCountryId = model.SupplierCountryId,
                Scope = model.Scope,
                ContactName = model.ContactName,
                ContactPhone = model.ContactPhone,
                ContactEmail = model.ContactEmail,
                ContactCountryId = model.ContactCountryId,
                Comments = "",
                Website = model.Website ?? "",
                RequestNum = ShortName + "-" + uniqueCode,
                CreatedBy = loggedInUserId,
                CreatedOn = DateTime.Now,
                StatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRPND)),
                BusinessRegistrationNo = model.BusinessRegistrationNo
            };
            _vendorRepository.Add(obj);
            _vendorRepository.SaveChanges();

            if (!await VendorAssignToManagers(obj.Id, loggedInUserId))
            {
                return 3;
            }

            return 1;
        }

        /// <summary>
        /// Assigns a vendor to managers.
        /// </summary>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <param name="WorkFlowStepId">The ID of the workflow step.</param>
        /// <param name="WrokStatus">The status of the work.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public async Task<bool> VendorAssignToManagers(Guid vendorId, string loggedInUserId, Guid WorkFlowStepId = default, Guid WrokStatus = default)
        {
            var WAFStatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), DropDownValuesEnum.WAF.GetStringValue());
            var approvedStatusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), DropDownValuesEnum.APPROVED.GetStringValue());
            var vendor = await _vendorRepository.Where(x => x.Id == vendorId).FirstOrDefaultAsync();
            if (vendor == null || WAFStatusId == Guid.Empty || approvedStatusId == Guid.Empty)
                return false;

            var previousStatusInfo = "";
            try
            {
                var workFlows = await _workflows.GetWorkFlows();
                var workFlowToProceed = workFlows.FirstOrDefault(x => x.Name == "Vendor Initialization Workflow");

                var workFlowSteplist = _VendorQualificationWorkFlowrepo.GetAll()
                                        .Where(x => x.IsActive && x.WorkFlowsId == workFlowToProceed?.Id)
                                        .OrderBy(x => x.StepOrder)
                                        .ToList();

                VendorQualificationWorkFlow workFlowStep;

                if (WorkFlowStepId != Guid.Empty)
                {
                    var vQWrokFlowExecution = await _zenithDbContext.VendorQualificationWorkFlowExecution.FirstOrDefaultAsync(x => x.VendorsInitializationFormId == vendorId && x.IsActive && x.VendorQualificationWorkFlowId == WorkFlowStepId);
                    var workstatusById = await _IDropdownList.GetDropDownValuById(WrokStatus);
                    var previousRequestStatus = string.IsNullOrEmpty(workstatusById) ? await _IDropdownList.GetDropDownValuById(vendor.StatusId) : workstatusById;
                    previousStatusInfo = string.IsNullOrEmpty(previousRequestStatus) ? "" : $"Previous work status is :: {previousRequestStatus},";

                    var workflowById = await _VendorQualificationWorkFlowrepo
                        .Where(x => x.Id == WorkFlowStepId)
                        .FirstOrDefaultAsync();

                    if (workflowById == null)
                    {
                        vendor.StatusId = WAFStatusId;
                        vendor.RequestStatusDescription = previousStatusInfo + "Failure reason :: Workflow assignment failed due to current workflow step not found in the system.";
                        await _vendorRepository.SaveChangesAsync();
                        return false;
                    }

                    var lastAssignedWorkFlowIndex = workFlowSteplist.IndexOf(workflowById);

                    // Check if the last workflow step is reached
                    if (lastAssignedWorkFlowIndex == workFlowSteplist.Count - 1)
                    {
                        vendor.IsApproved = true;
                        vendor.StatusId = approvedStatusId;
                        await _vendorRepository.SaveChangesAsync();
                        return true; // No further assignment needed
                    }

                    workFlowStep = workFlowSteplist[(lastAssignedWorkFlowIndex + 1) % workFlowSteplist.Count];

                }
                else
                {
                    workFlowStep = workFlowSteplist.FirstOrDefault();

                    var virURstatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRUR));
                    if (virURstatusId == Guid.Empty)
                    {
                        vendor.StatusId = WAFStatusId;
                        vendor.RequestStatusDescription = previousStatusInfo + "Failure reason :: Dropdownvalue is not found.";
                        await _vendorRepository.SaveChangesAsync();
                        return false;
                    }

                    vendor.StatusId = virURstatusId;
                    await _vendorRepository.SaveChangesAsync();
                }

                if (workFlowStep == null)
                {
                    vendor.StatusId = WAFStatusId;
                    vendor.RequestStatusDescription = previousStatusInfo + "Failure reason :: Next WorkFlow step not found.";
                    await _vendorRepository.SaveChangesAsync();
                    return false;
                }

                var role = await _roleManager.FindByIdAsync(workFlowStep.RoleId.ToString());
                if (role == null)
                {
                    vendor.StatusId = WAFStatusId;
                    vendor.RequestStatusDescription = previousStatusInfo + $"Failure reason :: Configured role for the workflow steps not found in the system";
                    await _vendorRepository.SaveChangesAsync();
                    return false;
                }

                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name ?? "");
                if (usersInRole == null || !usersInRole.Any())
                {
                    vendor.StatusId = WAFStatusId;
                    vendor.RequestStatusDescription = previousStatusInfo + $"Failure reason :: Users not found for the role({role?.Name ?? ""}).";
                    await _vendorRepository.SaveChangesAsync();
                    return false;
                }

                var userIds = usersInRole.Where(x => x.IsActive && x.IsVocationModeOn == false).OrderBy(x => x.FullName).Select(u => u.Id).ToList();

                var lastAssignedUser = _zenithDbContext.VendorQualificationWorkFlowExecution
                    .Where(execution => userIds.Contains(execution.AssignedUserId))
                    .OrderByDescending(execution => execution.CreatedOn)
                    .Select(execution => execution.AssignedUserId)
                    .FirstOrDefault();

                string nextAssignedUserId;

                if (lastAssignedUser == null)
                {
                    nextAssignedUserId = userIds.FirstOrDefault();
                }
                else
                {
                    var lastAssignedUserIndex = userIds.IndexOf(lastAssignedUser);

                    nextAssignedUserId = userIds[(lastAssignedUserIndex + 1) % userIds.Count];
                }

                if (string.IsNullOrEmpty(nextAssignedUserId))
                {
                    vendor.StatusId = WAFStatusId;
                    vendor.RequestStatusDescription = previousStatusInfo + "Failure reason ::No user found for Assigned vendor.";
                    await _vendorRepository.SaveChangesAsync();
                    return false;
                }

                var dropdownvalue = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.PND));

                if (dropdownvalue == Guid.Empty)
                {
                    vendor.StatusId = WAFStatusId;
                    vendor.RequestStatusDescription = previousStatusInfo + "Failure reason :: Dropdownvalue is not found.";
                    await _vendorRepository.SaveChangesAsync();
                    return false;
                }

                var record = new VendorQualificationWorkFlowExecutionDTO
                {
                    VendorQualificationWorkFlowId = workFlowStep.Id,
                    AssignedUserId = nextAssignedUserId,
                    IsActive = true,
                    VendorsInitializationFormId = vendorId,
                    StatusId = dropdownvalue,

                };

                await _vendorQualificationWorkFlowExecution.AddVendorQualificationWorkFlowExecution(record, loggedInUserId);
                return true;
            }
            catch (Exception)
            {
                _zenithDbContext.ChangeTracker.Clear();
                vendor.StatusId = WAFStatusId;
                vendor.RequestStatusDescription = "An issue occurred while assigning the vendor to the manager.";
                await _vendorRepository.UpdateAsync(vendor);
                return false;
            }
        }

        /// <summary>
        /// Updates an existing vendor.
        /// </summary>
        /// <param name="model">The update vendor DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public async Task<string> UpdateVendor(updateVendorDTO model, string loggedInUserId)
        {
            var vendor = await _vendorRepository.Where(x => x.Id == model.VendorsInitializationFormId).FirstOrDefaultAsync();

            if (vendor != null)
            {
                var vendorQualificationworkFlowExexution = await _zenithDbContext.VendorQualificationWorkFlowExecution.FirstOrDefaultAsync(x => x.VendorsInitializationFormId == model.VendorsInitializationFormId && x.IsActive);
                var completeId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.COMPLETED));

                if (model.IsApproved || model.IsSubmitForApproval)
                {
                    var VIRApprovedId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRAPRVD));

                    if (vendorQualificationworkFlowExexution != null && VIRApprovedId != Guid.Empty)
                    {
                        var virURstatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRUR));
                        if (vendor.StatusId == virURstatusId)
                        {
                            if (await IsDuplicateBusinesReqNoCombinetion(vendor))
                            {
                                var virRejectDTDuplicate = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRCANDTDUPLI));
                                vendor.StatusId = virRejectDTDuplicate;

                                string supplierName = vendor.SupplierName ?? "N/A";
                                string countryName = _zenithDbContext.DropdownValues.FirstOrDefault(x => x.Id == vendor.SupplierCountryId).Value ?? "N/A";
                                string registrationNumber = vendor.BusinessRegistrationNo ?? "N/A";

                                string body = @$"Request is cancelled due to duplicates in the system. 
                                                Please enter the correct details. Please check the following details: 
                                                <br>
                                                - Supplier Name: {supplierName} <br>
                                                - Registered Country: {countryName} <br>
                                                - Business Registration Number: {registrationNumber}";

                                var createby = await _userManager.FindByIdAsync(vendor.CreatedBy);
                                if (createby.Email != null && !string.IsNullOrEmpty(body))
                                {
                                    _emailUtils.SendMail("KdSolution@gmail.com", createby.Email, body);
                                }

                                var vendorQualificationworkFlow = await _zenithDbContext.VendorQualificationWorkFlowExecution.FirstOrDefaultAsync(x => x.VendorsInitializationFormId == model.VendorsInitializationFormId && x.IsActive);
                                vendorQualificationworkFlow.IsActive = false;
                                await _zenithDbContext.SaveChangesAsync();

                                return "";
                            }

                            vendor.StatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRPND));
                        }

                        if (!await VendorAssignToManagers(model.VendorsInitializationFormId, loggedInUserId, vendorQualificationworkFlowExexution.VendorQualificationWorkFlowId, model.IsApproved ? VIRApprovedId : completeId))
                        {
                            var vendorQualificationworkFlow = await _zenithDbContext.VendorQualificationWorkFlowExecution.FirstOrDefaultAsync(x => x.VendorsInitializationFormId == model.VendorsInitializationFormId && x.IsActive);
                            vendorQualificationworkFlow.IsActive = false;
                            vendorQualificationworkFlow.StatusId = model.IsApproved? VIRApprovedId: completeId;
                            _zenithDbContext.VendorQualificationWorkFlowExecution.Update(vendorQualificationworkFlow);
                            await _zenithDbContext.SaveChangesAsync();

                            return "";
                        }

                        vendorQualificationworkFlowExexution.IsActive = false;
                        vendorQualificationworkFlowExexution.StatusId = model.IsApproved ? VIRApprovedId : completeId;
                    }


                    var workflowById = await _VendorQualificationWorkFlowrepo
                        .Where(x => x.Id == vendorQualificationworkFlowExexution.VendorQualificationWorkFlowId)
                        .FirstOrDefaultAsync();

                    if (workflowById != null && workflowById.StepName == "Manager Workbench")
                    {
                        var VQFPNDId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VQFPND));
                        vendor.StatusId = VQFPNDId;
                    }
                }
                else
                {
                    if (model.RejectionReasonId != Guid.Empty)
                    {
                        var rejected = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRRJCTD));
                        vendor.RejectionReasonId = model.RejectionReasonId;
                        vendor.StatusId = rejected;

                        if (vendorQualificationworkFlowExexution != null && rejected != Guid.Empty)
                        {
                            vendorQualificationworkFlowExexution.IsActive = false;
                            vendorQualificationworkFlowExexution.StatusId = rejected;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(model.comments))
                {
                    vendor.Comments = model.comments;
                }

                _zenithDbContext.VendorQualificationWorkFlowExecution.Update(vendorQualificationworkFlowExexution);
                await _zenithDbContext.SaveChangesAsync();

                await _vendorRepository.UpdateAsync(vendor);
                return "ok";
            }

            return "Something went wrong";
        }

        /// <summary>
        /// Updates the critical/non-critical status of a vendor.
        /// </summary>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <param name="isVendorCritical">The critical status of the vendor.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public async Task<bool> UpdateVendorCriticalNonCritical(Guid vendorId, bool isVendorCritical)
        {
            var vendor = await _vendorRepository.Where(x => x.Id == vendorId).FirstOrDefaultAsync();

            if (vendor != null)
            {
                vendor.IsCritical = isVendorCritical;
                await _vendorRepository.UpdateAsync(vendor);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a vendor has a duplicate business registration number combination.
        /// </summary>
        /// <param name="model">The vendor initialization form model.</param>
        /// <returns>A boolean indicating whether a duplicate exists.</returns>
        public async Task<bool> IsDuplicateBusinesReqNoCombinetion(VendorsInitializationForm model)
        {
            var virRejectstatusId = (await _zenithDbContext.DropdownValues.FirstOrDefaultAsync(x => x.Code == "VIRRJCTD"))?.Id ?? Guid.Empty;
            var virCancelledstatusId = (await _zenithDbContext.DropdownValues.FirstOrDefaultAsync(x => x.Code == "VIRCANDTDUPLI"))?.Id ?? Guid.Empty;

            var duplicateCount = _vendorRepository.Where(x
                                    => x.BusinessRegistrationNo == model.BusinessRegistrationNo
                                    && x.SupplierName == model.SupplierName
                                    && x.SupplierCountryId == model.SupplierCountryId
                                    && (x.IsActive || (x.StatusId != virRejectstatusId && x.StatusId != virCancelledstatusId))).Count();
            return await Task.FromResult(duplicateCount > 1);
        }

        /// <summary>
        /// Updates the details of a vendor.
        /// </summary>
        /// <param name="model">The vendor DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public async Task<bool> UpdateVendorDetails(VendorDTO model, string loggedInUserId)
        {
            try
            {
                var vendor = await _vendorRepository.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

                if (vendor == null)
                    return false;

                var virURstatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRUR));
                if (vendor.StatusId == virURstatusId)
                {
                    if (await IsDuplicateBusinesReqNoCombinetion(vendor))
                    {
                        var virRejectDTDuplicate = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRCANDTDUPLI));
                        vendor.StatusId = virRejectDTDuplicate;

                        string supplierName = vendor.SupplierName ?? "N/A";
                        string countryName = _zenithDbContext.DropdownValues.FirstOrDefault(x => x.Id == vendor.SupplierCountryId).Value ?? "N/A";
                        string registrationNumber = vendor.BusinessRegistrationNo ?? "N/A";

                        string body = @$"Request is cancelled due to duplicates in the system. 
                                                Please enter the correct details. Please check the following details: 
                                                <br>
                                                - Supplier Name: {supplierName} <br>
                                                - Registered Country: {countryName} <br>
                                                - Business Registration Number: {registrationNumber}";

                        var createby = await _userManager.FindByIdAsync(vendor.CreatedBy);
                        if (createby.Email != null && !string.IsNullOrEmpty(body))
                        {
                            _emailUtils.SendMail("KdSolution@gmail.com", createby.Email, body);
                        }

                        var vendorQualificationworkFlow = await _zenithDbContext.VendorQualificationWorkFlowExecution.FirstOrDefaultAsync(x => x.VendorsInitializationFormId == vendor.Id && x.IsActive);
                        vendorQualificationworkFlow.IsActive = false;
                        await _zenithDbContext.SaveChangesAsync();

                        return true;
                    }
                    vendor.StatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRPND));
                }

                vendor.RequestedBy = model.RequestedBy;
                vendor.CreatedBy = model.CreatedBy;
                vendor.PriorityId = model.PriorityId;
                vendor.RequiredBy = model.RequiredBy;
                vendor.SupplierName = model.SupplierName;
                vendor.SupplierTypeId = model.SupplierTypeId;
                vendor.Scope = model.Scope;
                vendor.ContactName = model.ContactName;
                vendor.ContactPhone = model.ContactPhone;
                vendor.ContactEmail = model.ContactEmail;
                vendor.ContactCountryId = model.ContactCountryId;
                vendor.Website = model.Website;
                vendor.BusinessRegistrationNo = model.BusinessRegistrationNo;
                vendor.SupplierCountryId = model.SupplierCountryId;
                await _vendorRepository.UpdateAsync(vendor);
                return true;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Adds a new address.
        /// </summary>
        /// <param name="model">The address DTO.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        public int AddAddress(AddressDTO model)
        {
            Address obj = new Address
            {
                FullAddress = model.FullAddress,
                OfficeNo = model.OfficeNo,
                BuildingName = model.BuildingName,
                Street = model.Street,
                LocalTownId = model.LocalTownId,
                DistrictId = model.DistrictId,
                StateId = model.StateId,
                CountryId = model.CountryId,
                Code = model.Code,
                CodeTypeId = model.CodeTypeId,
                GeorgraphicLocationId = model.GeorgraphicLocationId
            };
            _addressRepository.Add(obj);
            _addressRepository.SaveChanges();
            return 1;
        }

        /// <summary>
        /// Adds a new registration.
        /// </summary>
        /// <param name="model">The registration DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string AddNewRegistration(RegistrationDTO model)
        {
            var alreadyRegister = _registrationRepository.Where(x => x.LicenseNumber == model.LicenseNumber).Any();

            if (alreadyRegister != true)
            {
                Registrations obj = new Registrations
                {
                    LicenseTypeId = model.LicenseTypeId,
                    LicenseNumber = model.LicenseNumber,
                    RegisteredSince = model.RegisteredSince,
                    RegistrationCategoryId = model.RegistrationCategoryId,
                    RegisteredCountryId = model.RegisteredCountryId,
                    RegistrationValidityId = model.RegistrationValidityId,
                    ValidityStart = model.ValidityStart,
                    ValidityEnd = model.ValidityEnd,
                    Document = "",
                    ActivityCode = model.ActivityCode,
                    ActivityNameId = model.ActivityName
                };
                _registrationRepository.Add(obj);
                _registrationRepository.SaveChanges();
            }
            else
            {
                return "Already register";
            }
            return "Ok";
        }

        /// <summary>
        /// Adds a new quality certification.
        /// </summary>
        /// <param name="model">The quality certification DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string AddQualityCertification(QualityCertificationDTO model)
        {
            QualityCertification obj = new QualityCertification
            {
                LicenseById = model.LicenseById,
                LicenseStandardId = model.LicenseStandardId,
                LicenseNumber = model.LicenseNumber,
                RegistrationValidityId = model.RegistrationValidityId,
                ValidityStart = model.ValidityStart,
                ValidityEnd = model.ValidityEnd,
                Document = "",
                LicenseScope = model.LicenseScope,
            };
            _qualityCertificationRepository.Add(obj);
            _qualityCertificationRepository.SaveChanges();

            return "ok";
        }

        /// <summary>
        /// Adds new payment terms.
        /// </summary>
        /// <param name="model">The payment terms DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string AddPaymentTerms(PaymentTermsDTO model)
        {
            return "ok";
        }

        /// <summary>
        /// Adds new account details.
        /// </summary>
        /// <param name="model">The account details DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string AddAccountDetails(AccountDetailsDTO model)
        {
            var alreadyAccount = _accountDetailRepository.Where(x => x.BankNameId == model.BankNameId && x.IfscCodeId == model.IfscCodeId && x.AccountNumber == model.AccountNumber).Any();

            if (alreadyAccount != true)
            {
                AccountDetails obj = new AccountDetails
                {

                    BankNameId = model.BankNameId,
                    BranchNameId = model.BranchNameId,
                    BankCountryId = model.BankCountryId,
                    AccountNumber = model.AccountNumber,
                    AccountCurrencyId = model.AccountCurrencyId,
                    IBANId = model.IBANId,
                    SwiftCodeId = model.SwiftCodeId,
                    IfscCodeId = model.IfscCodeId,
                    BankAddressId = model.BankAddressId,
                    BankLetter = "",
                    IsActive = true

                };
                _accountDetailRepository.Add(obj);
                _accountDetailRepository.SaveChanges();
            }
            else
            {
                return "Already register";
            }
            return "Ok";
        }

        /// <summary>
        /// Adds other documents.
        /// </summary>
        /// <param name="model">The other documents DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string AddOtherDocuments(OtherDocumentsDTO model)
        {
            var alreadyDoc = _otherRepository.Where(x => x.DocumentName == model.DocumentName).Any();

            if (alreadyDoc != true)
            {
                OtherDocuments obj = new OtherDocuments
                {
                    DocumentName = model.DocumentName,
                    DocumentTypeId = model.DocumentTypeId,
                    Document = "",
                    ExpiryDateId = model.ExpiryDateId,
                };
                _otherRepository.Add(obj);
                _otherRepository.SaveChanges();
            }
            else
            {
                return "Already doc";
            }
            return "Ok";
        }

        /// <summary>
        /// Generates a unique code.
        /// </summary>
        /// <returns>A generated unique code string.</returns>
        public string GenerateUniqueCode()
        {
            string code;
            Random rand = new Random();
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

            code = string.Empty;
            for (int i = 0; i < 7; i++)
            {
                code += saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
            }
            return code;
        }

        /// <summary>
        /// Generates a short name from a full name.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <returns>A generated short name string.</returns>
        public string GenerateShortName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;
            var words = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var shortName = string.Concat(words.Select(word => word[0].ToString().ToUpper()));
            return shortName;
        }
    }
}
