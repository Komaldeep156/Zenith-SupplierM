using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class SecurityGroupsDTO : SecurityGroups
    {
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
    }
}
