using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class VendorsLogic : IVendors
    {
        private readonly IRepository<Vendors> _vendorRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Registrations> _registrationRepository;
        private readonly IRepository<QualityCertification> _qualityCertificationRepository;
        private readonly IRepository<AccountDetails> _accountDetailRepository;
        private readonly IRepository<OtherDocuments> _otherRepository;
        private readonly RoleManager<IdentityRole> _roleManager;

        public VendorsLogic(IRepository<Vendors> vendorRepository, IRepository<Address> AddressRepository,
            IRepository<Registrations> RegistrationRepository, IRepository<QualityCertification> QualityCertificationRepository,
            IRepository<AccountDetails> accountDetailRepository, IRepository<OtherDocuments> otherRepository,
            RoleManager<IdentityRole> roleManager )
        {
            _vendorRepository = vendorRepository;
            _addressRepository = AddressRepository;
            _registrationRepository = RegistrationRepository;
            _qualityCertificationRepository = QualityCertificationRepository;
            _accountDetailRepository = accountDetailRepository;
            _otherRepository = otherRepository;
            _roleManager = roleManager;
        }

        public List<GetVendorsListDTO> GetVendors()
        {
            var data = (from a in _vendorRepository
                        where a.IsDeleted == false
                        select new GetVendorsListDTO
                        {
                            Id  = a.Id,
                            SupplierCode = a.SupplierCode,
                            FullName = a.FullName,
                            ShortName = a.ShortName,
                            Website = a.Website,
                            SupplierCategory = a.DropdownValues_SupplierCategory,
                            SupplierScope = a.DropdownValues_SupplierScope,
                            RevisionNumber = a.RevisionNumber,
                            ApprovalStatus = a.ApprovalStatus,
                            RejectionReason = a.RejectionReason,
                            IsActive = a.IsActive,
                            
                        }).ToList();
            return data;
        }
        public GetVendorsListDTO GetVendorById(Guid vendorId)
        {
            var vendor = (from a in _vendorRepository
                          where a.Id == vendorId
                          select new GetVendorsListDTO
                          {
                              Id = a.Id,
                              SupplierCode = a.SupplierCode,
                              FullName = a.FullName,
                              ShortName = a.ShortName,
                              Website = a.Website,
                              AssignedTo = "",
                              RevisionNumber = a.RevisionNumber,
                              ApprovalStatus = a.ApprovalStatus,
                              RejectionReason = a.RejectionReason,
                              IsActive = a.IsActive
                          }).FirstOrDefault();

            return vendor;
        }
        public List<GetVendorsListDTO> SearchVendorList(string fieldName, string searchText)
        {
            var dataList = _vendorRepository.Include(x => x.DropdownValues_SupplierCategory)
                                        .Include(x => x.DropdownValues_SupplierScope)
                                        .Where(x => !x.IsDeleted)
                                        .ToList();


            var data = dataList.AsQueryable(); 

            if (!string.IsNullOrEmpty(searchText))
            {
                switch (fieldName.ToLower())
                {
                    case "fullname":
                        data = data.Where(x => x.FullName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "suppliercode":
                        data = data.Where(x => x.SupplierCode.Contains(searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    default:
                        break;
                }
            }

            return data.Select(a => new GetVendorsListDTO
            {
                Id = a.Id,
                SupplierCode = a.SupplierCode,
                FullName = a.FullName,
                ShortName = a.ShortName,
                Website = a.Website,
                SupplierCategory = a.DropdownValues_SupplierCategory,
                SupplierScope = a.DropdownValues_SupplierScope,
                RevisionNumber = a.RevisionNumber,
                ApprovalStatus = a.ApprovalStatus,
                RejectionReason = a.RejectionReason,
                IsActive = a.IsActive,
            }).ToList();
        }
        public int AddVendor(VendorDTO model, Guid tenantId)
        {
            var role = _roleManager.FindByNameAsync("Vendor Manager").Result;
            if (role != null && role.Id != null)
            {
                string code = GenerateUniqueCode();
                string ShortName = GenerateShortName(model.FullName);
                Vendors obj = new Vendors
                {
                    SupplierCode = "S-" + Convert.ToInt32(code),
                    FullName = model.FullName,
                    ShortName = ShortName,
                    Website = model.Website,
                    SupplierCategoryId = model.SupplierCategoryId,
                    SupplierScopeId = model.SupplierScopeId,
                    AssignedRoleId = role.Id,
                    IsActive = false,
                    TenantId = tenantId,
                    ApprovalStatus = "pending",
                    RejectionReason = "",
                    RevisionNumber = 1,
                    CreatedOn = DateTime.Now,
                    CreatedBy = model.SupplierCategoryId,
                    ModifiedBy = model.SupplierCategoryId,
                    ModifiedOn = DateTime.Now,

                };
                _vendorRepository.Add(obj);
                _vendorRepository.SaveChanges();
            }
            
            return 1;
        }
        public async Task<string> UpdateVendor(updateVendorDTO model)
        {
            var vendor = _vendorRepository.Where(x => x.Id == model.vendorId).FirstOrDefault();

            if (vendor != null && model.IsApproved && !model.IsCriticalApproved)
            {
                vendor.ApprovalStatus = "approved";
            }

            if(vendor != null && !model.IsApproved && model.IsCriticalApproved)
            {
                vendor.ApprovalStatus = "approved";
                vendor.IsCriticalApproved = true;
            }

            if (vendor != null && !model.IsApproved && !model.IsCriticalApproved)
            {
                vendor.ApprovalStatus = "reject";
                vendor.RejectionReason = model.RejectionReason;
            }

            if(vendor != null && model.AssignedRoleId != null)
            {
                vendor.AssignedRoleId = model.AssignedRoleId;
            }


            if (vendor != null)
            {
                vendor.FullName = model.FullName;
                vendor.Website = model.Website;
                vendor.SupplierCategoryId = model.SupplierCategoryId;
                vendor.SupplierScopeId = model.SupplierScopeId;

                await _vendorRepository.UpdateAsync(vendor);
                return "ok";
            }

            return "Something went wrong";
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
