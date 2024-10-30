using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IManufacturer
    {
        public List<GetManufactureListDTO> getManufacture();
        public GetManufactureListDTO GetManufacturerById(Guid ManufacturerId);
        Guid AddManufacturer(ManufacturerDTO model);


        Guid AddNewBrand(BrandDTO model);
        Guid AddNewProduct(ProductDTO model);
    }
}
