using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IFields
    {
        /// <summary>
        /// Retrieves a list of fields based on the provided filters.
        /// </summary>
        /// <param name="fieldId">The ID of the field.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="searchText">The search text to filter fields.</param>
        /// <returns>A list of field DTOs.</returns>
        Task<List<FieldsDTO>> GetAllFields(Guid? fieldId = null, string fieldName = null, string searchText = null);

        /// <summary>
        /// Adds a new field.
        /// </summary>
        /// <param name="model">The field DTO.</param>
        Task AddFields(FieldsDTO model);

        /// <summary>
        /// Deletes a field by its ID.
        /// </summary>
        /// <param name="fieldId">The ID of the field.</param>
        Task DeleteField(Guid fieldId);
    }
}
