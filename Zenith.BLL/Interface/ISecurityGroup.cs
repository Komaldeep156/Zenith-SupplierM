using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface ISecurityGroup
    {
        Task<List<SecurityGroupsDTO>> GetAllSecurityGroups(Guid? securityGroupId = null, string name = null, string securityGroupCode = null);
        Task AddSecurityGroup(SecurityGroupsDTO model);
        Task<(bool isSuccess, List<string> notDeletedSecurityGroupNames)> DeleteSecurityGroup(List<Guid> securityGroupIds);
        Task<(bool isSuccess, List<string> notUpdatedSecurityGroupNames)> UpdateSecurityGroupToActive(List<Guid> securityGroupIds, bool IsActive);
    }
}
