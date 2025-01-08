using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IFields
    {
        Task<List<FieldsDTO>> GetAllfields(Guid? fieldId = null, string fieldName = null, string searchText = null);
        Task AddFields(FieldsDTO model);
        Task DeleteField(Guid fieldId);
    }
}
