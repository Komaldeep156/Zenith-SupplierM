using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class FieldsDTO : Fields
    {
        public Guid? FieldId { get; set; }
        public String CreatedByName { get; set; }
        public String ModifiedByName { get; set; }

    }
}
