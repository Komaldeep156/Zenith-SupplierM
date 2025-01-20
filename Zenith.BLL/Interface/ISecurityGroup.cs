using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface ISecurityGroup
    {
        Task<List<SecurityGroupsDTO>> GetAllSecurityGroups(Guid? securityGroupId = null, string name = null, string securityGroupCode = null);
        Task<Guid> AddSecurityGroup(SecurityGroupsDTO model);
        Task<(bool isSuccess, List<string> notDeletedSecurityGroupNames)> DeleteSecurityGroup(List<Guid> securityGroupIds);
        Task<(bool isSuccess, List<string> notCopySecurityGroupNames)> CopySecurityGroup(List<Guid> securityGroupIds, string loginUserId);
        Task<(bool isSuccess, List<string> notUpdatedSecurityGroupNames)> UpdateSecurityGroupToActive(List<Guid> securityGroupIds, bool IsActive);
        Task<int> IsDuplicateSecurityGroup(SecurityGroupsDTO model);
        Task<List<SecurityGroupFields>> GetSecurityGroupFieldsIdBySecurityGroupId(Guid securityGroupId);
        Task<SecurityGroupsDTO> GetSecurityGroupsById(Guid? securityGroupId = null);
        Task UpdateSecurityGroup(SecurityGroupsDTO model);
    }
}
