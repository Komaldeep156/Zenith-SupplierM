using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface ISecurityGroupUsersLogic
    {
        Task<SecurityGroupUsers> AddSecurityGroupUsers(SecurityGroupUsersDTO model);
        Task<List<string>> GetAssignedUserIdsBySecurityGroupId(Guid securityGroupId);
        Task RemoveSecurityGroupUsers(Guid SecurityGroupId);
        Task RemoveAllSecurityGroupUsersAssignedToUserID(string userId);
    }
}
