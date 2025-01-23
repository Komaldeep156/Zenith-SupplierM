using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface ISecurityGroupUsersLogic
    {
        /// <summary>
        /// Adds a new security group user.
        /// </summary>
        /// <param name="model">The security group user DTO.</param>
        /// <returns>The newly created security group user.</returns>
        Task<SecurityGroupUsers> AddSecurityGroupUsers(SecurityGroupUsersDTO model);
        
        /// <summary>
        /// Removes security group users by security group ID.
        /// </summary>
        /// <param name="SecurityGroupId">The ID of the security group.</param>
        Task RemoveSecurityGroupUsers(Guid SecurityGroupId);
        
        /// <summary>
        /// Removes all security group users assigned to a specific user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        Task RemoveAllSecurityGroupUsersAssignedToUserID(string userId);
        
        /// <summary>
        /// Retrieves a list of user IDs assigned to a specific security group.
        /// </summary>
        /// <param name="securityGroupId">The ID of the security group.</param>
        /// <returns>A list of user IDs.</returns>
        Task<List<string>> GetAssignedUserIdsBySecurityGroupId(Guid securityGroupId);

        
        
    }
}
