using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class ManufacturerLogic : IManufacturer
    {
        private IRepository<Manufacturer> _manufacturerRepository;
        private IRepository<Brands> _brandRepository;
        private IRepository<Products> _productRepository;

        public ManufacturerLogic(IRepository<Manufacturer> manufacutrerRepository,
            IRepository<Brands> brandRepository, IRepository<Products> productRepository)
        {
            _manufacturerRepository = manufacutrerRepository;
            _brandRepository = brandRepository;
            _productRepository = productRepository;
        }

        public List<GetManufactureListDTO> getManufacture()
        {
            var data = (from a in _manufacturerRepository
                        where a.IsDeleted == false
                        select new GetManufactureListDTO
                        {
                            Id = a.Id,
                            ManufacturerCode = a.ManufacturerCode,
                            FullName = a.FullName,
                            ShortName = a.ShortName,
                            Website = a.Website,
                            RegisteredSince = a.DropdownValues_RegisteredSince,
                            HeadQuarter = a.DropdownValues_HeadQuarter,
                            RevisionNumer = a.RevisionNumer,
                            ApprovalStatus = a.ApprovalStatus,
                            RejectionReason = a.RejectionReason,
                            IsActive = a.IsActive,

                        }).ToList();
            return data;
        }

        public GetManufactureListDTO GetManufacturerById(Guid ManufacturerId)
        {
            var manufaturer = (from a in _manufacturerRepository
                          where a.Id == ManufacturerId
                               select new GetManufactureListDTO
                          {
                              Id = a.Id,
                              ManufacturerCode = a.ManufacturerCode,
                              FullName = a.FullName,
                              ShortName = a.ShortName,
                              Website = a.Website,
                              RegisteredSince = a.DropdownValues_RegisteredSince,
                              HeadQuarter = a.DropdownValues_HeadQuarter,
                              RevisionNumer = a.RevisionNumer,
                              ApprovalStatus = a.ApprovalStatus,
                              RejectionReason = a.RejectionReason,
                              IsActive = a.IsActive,
                          }).FirstOrDefault();

            return manufaturer;
        }

        public Guid AddManufacturer(ManufacturerDTO model)
        {
            string code = GenerateUniqueCode();
            string ShortName = GenerateShortName(model.FullName);
            Manufacturer obj = new Manufacturer
            {
                ManufacturerCode = "M-" + Convert.ToInt32(code),
                FullName = model.FullName,
                ShortName = ShortName,
                Website = model.Website,
                RegisteredSinceId = model.RegisteredSinceId,
                HeadQuarterId = model.HeadQuarterId,
                ApprovalStatus = "",
                IsActive = true,
                RejectionReason = "",
                RevisionNumer = 1

            };
            _manufacturerRepository.Add(obj);
            _manufacturerRepository.SaveChanges();
            return obj.Id;
        }
        public Guid AddNewBrand(BrandDTO model)
        {
            string code = GenerateUniqueCode();
            string ShortName = GenerateShortName(model.BrandName);
            Brands obj = new Brands
            {
                BrandCode = "BC-" + Convert.ToInt32(code),
                FullName = model.BrandName,
                ShortName = ShortName,
                LogoFilePath = ""

            };
            _brandRepository.Add(obj);
            _brandRepository.SaveChanges();
            return obj.Id;
        }
        public Guid AddNewProduct(ProductDTO model)
        {
            string code = GenerateUniqueCode();
            Products obj = new Products
            {
                ProductFamilyCode = "PC-" + Convert.ToInt32(code),
                ProductNameCode = "PN-" + Convert.ToInt32(code),
                ProductNameId = model.ProductNameId,
                ProductFamilyNameId = model.ProductFamilyNameId,

            };
            _productRepository.Add(obj);
            _productRepository.SaveChanges();
            return obj.Id;
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
