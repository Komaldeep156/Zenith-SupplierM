﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
using Zenith.Repository.RepositoryFiles;

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

        public VendorsLogic(IRepository<VendorsInitializationForm> vendorRepository, IRepository<Address> AddressRepository,
            IRepository<Registrations> RegistrationRepository, IRepository<QualityCertification> QualityCertificationRepository,
            IRepository<AccountDetails> accountDetailRepository, IRepository<OtherDocuments> otherRepository,
            RoleManager<IdentityRole> roleManager, IDropdownList iDropdownList)
        {
            _vendorRepository = vendorRepository;
            _addressRepository = AddressRepository;
            _registrationRepository = RegistrationRepository;
            _qualityCertificationRepository = QualityCertificationRepository;
            _accountDetailRepository = accountDetailRepository;
            _otherRepository = otherRepository;
            _roleManager = roleManager;
            _IDropdownList = iDropdownList;
        }

        public List<GetVendorsListDTO> GetVendors()
        {

            var data = (from a in _vendorRepository
                        where !a.IsDeleted 
                        select new GetVendorsListDTO
                        {
                            Id = a.Id,
                            SupplierName = a.SupplierName,
                            RequestNum = a.RequestNum,
                            DropdownValues_Priority = a.DropdownValues_Priority,
                            RequiredBy = a.RequiredBy,
                            DropdownValues_SupplierType = a.DropdownValues_SupplierType,
                            Scope = a.Scope,
                            ContactName = a.ContactName,
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
                            IsDelgateRequested = a.DropdownValues_Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue(),
                        }).ToList();
            return data;
        }

        public bool DeleteVendors(List<Guid> selectedVendorIds)
        {
            if (selectedVendorIds!=null)
            {
                foreach (var vendor in selectedVendorIds)
                {

                    var dbVendor = _vendorRepository.Where(x => x.Id == vendor).FirstOrDefault();
                    if (dbVendor!=null)
                    _vendorRepository.Remove(dbVendor);
                    _vendorRepository.SaveChanges();
                }
            }
            return true;
        }
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
                              DropdownValues_SupplierType = a.DropdownValues_SupplierType,
                              Scope = a.Scope,
                              ContactName = a.ContactName,
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
                          }).FirstOrDefault();

            return vendor;
        }
        public List<GetVendorsListDTO> SearchVendorList(string fieldName, string searchText)
        {
            var data = (from a in _vendorRepository
                        where !a.IsDeleted && a.DropdownValues_Status != null
            && (a.DropdownValues_Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue()
            || a.DropdownValues_Status.Value == DropDownValuesEnum.PENDING.GetStringValue())
                        select new GetVendorsListDTO
                        {
                            Id = a.Id,
                            SupplierName = a.SupplierName,
                            RequestNum = a.RequestNum,
                            DropdownValues_Priority = a.DropdownValues_Priority,
                            RequiredBy = a.RequiredBy,
                            DropdownValues_SupplierType = a.DropdownValues_SupplierType,
                            Scope = a.Scope,
                            ContactName = a.ContactName,
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
                            IsDelgateRequested = a.DropdownValues_Status.Value == DropDownValuesEnum.DelegateRequested.GetStringValue(),
                        }).ToList();

            if (!string.IsNullOrEmpty(searchText))
            {
                switch (fieldName)
                {
                    case "supplierName":
                        data = data.Where(x => x.SupplierName.Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "requestNo":
                        data = data.Where(x => x.RequestNum.Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "supplierCountry":
                        data = data.Where(x => x.DropdownValues_SupplierCountry != null && x.DropdownValues_SupplierCountry.Value.Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    default:
                        break;
                }
            }

            return data;


        }
        public int AddVendor(VendorDTO model, string loggedInUserId)
        {
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
                StatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRPND))
            };
            _vendorRepository.Add(obj);
            _vendorRepository.SaveChanges();
            return 1;
        }
        public async Task<string> UpdateVendor(updateVendorDTO model)
        {
            var vendor = await _vendorRepository.Where(x => x.Id == model.VendorsInitializationFormId).FirstOrDefaultAsync();

            if (vendor != null)
            {
                if (model.IsApproved)
                {
                    vendor.IsApproved = true;
                    vendor.StatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRAPRVD));
                }
                else
                {
                    if (model.RejectionReasonId != Guid.Empty)
                    {
                        vendor.RejectionReasonId = model.RejectionReasonId;
                        vendor.StatusId = _IDropdownList.GetIdByDropdownCode(nameof(DropDownListsEnum.STATUS), nameof(DropDownValuesEnum.VIRRJCTD));
                    }
                }

                if (!string.IsNullOrEmpty(model.comments))
                {
                    vendor.Comments = model.comments;
                }

                await _vendorRepository.UpdateAsync(vendor);
                return "ok";
            }

            return "Something went wrong";
        }
        public async Task<bool> UpdateVendorCriticalNonCritical(Guid vendorId, bool isVendorCritical)
        {
            var vendor = await _vendorRepository.Where(x => x.Id == vendorId).FirstOrDefaultAsync();

            if (vendor != null)
            {
                vendor.IsCritical = isVendorCritical;
                await _vendorRepository.UpdateAsync(vendor);
                return true;
            }
            return  false;
        }

        public async Task<bool> UpdateVendorStatuses(List<string> vendorIds, string status)
        {
            if (vendorIds == null || !vendorIds.Any() || string.IsNullOrEmpty(status))
            {
                return false;
            }

            Guid statusId = _IDropdownList.GetIdByDropdownValue(nameof(DropDownListsEnum.STATUS), status);
            if (statusId == Guid.Empty)
            {
                return false;
            }

            // Convert vendorIds to GUID and retrieve all vendors in a single query
            var vendorGuidIds = vendorIds.Select(id => new Guid(id)).ToList();
            var vendors = await _vendorRepository.Where(x => vendorGuidIds.Contains(x.Id)).ToListAsync();

            if (!vendors.Any())
            {
                return false;
            }

            // Update the status of each vendor in memory
            foreach (var vendor in vendors)
            {
                vendor.StatusId = statusId;
            }

            // Save all changes at once
            await _vendorRepository.SaveChangesAsync();

            return true;
        }


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
        public string AddPaymentTerms(PaymentTermsDTO model)
        {
            return "ok";
        }
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
        public string GenerateShortName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return string.Empty;
            var words = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var shortName = string.Concat(words.Select(word => word[0].ToString().ToUpper()));
            return shortName;
        }
    }
}
