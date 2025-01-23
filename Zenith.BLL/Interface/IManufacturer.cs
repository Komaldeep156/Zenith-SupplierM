using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IManufacturer
    {
        /// <summary>
        /// Retrieves a list of manufacturers.
        /// </summary>
        /// <returns>A list of manufacturer DTOs.</returns>
        public List<GetManufactureListDTO> getManufacture();

        /// <summary>
        /// Retrieves a manufacturer by its ID.
        /// </summary>
        /// <param name="ManufacturerId">The ID of the manufacturer.</param>
        /// <returns>A manufacturer DTO.</returns>
        public GetManufactureListDTO GetManufacturerById(Guid ManufacturerId);

        /// <summary>
        /// Adds a new manufacturer.
        /// </summary>
        /// <param name="model">The manufacturer DTO.</param>
        /// <returns>The ID of the newly created manufacturer.</returns>
        Guid AddManufacturer(ManufacturerDTO model);

        /// <summary>
        /// Adds a new brand.
        /// </summary>
        /// <param name="model">The brand DTO.</param>
        /// <returns>The ID of the newly created brand.</returns>
        Guid AddNewBrand(BrandDTO model);

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="model">The product DTO.</param>
        /// <returns>The ID of the newly created product.</returns>
        Guid AddNewProduct(ProductDTO model);
    }
}
