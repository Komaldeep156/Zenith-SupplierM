using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class SecurityGroupsDTO : SecurityGroups
    {
        public SecurityGroupsDTO()
        {
            Fields = new List<FieldsDTO>();
        }

        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public List<FieldsDTO> Fields { get; set; }
        public List<SecurityGroupFieldsDTO> SecurityGroupFieldsDTOList { get; set; }
        public List<Guid> AssignedUserIds { get; set; }
    }
}
