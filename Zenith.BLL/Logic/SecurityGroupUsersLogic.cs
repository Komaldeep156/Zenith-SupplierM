using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class SecurityGroupUsersLogic : ISecurityGroupUsersLogic
    {
        private readonly ZenithDbContext _context;
        private readonly IRepository<SecurityGroupUsers> _securityGroupUserRepo;

        public SecurityGroupUsersLogic(ZenithDbContext context, IRepository<SecurityGroupUsers> groupUserRepo)
        {
            _context = context;
            _securityGroupUserRepo = groupUserRepo;
        }

        /// <summary>
        /// Adds a new security group user.
        /// </summary>
        /// <param name="model">The security group user DTO.</param>
        /// <returns>The newly created security group user.</returns>
        public async Task<SecurityGroupUsers> AddSecurityGroupUsers(SecurityGroupUsersDTO model)
        {
            if (model != null)
            {
                var result = await _context.SecurityGroupUsers.AddAsync(model);
                await _context.SaveChangesAsync();

                return model;
            }
            return null;
        }

        /// <summary>
        /// Removes security group users by security group ID.
        /// </summary>
        /// <param name="SecurityGroupId">The ID of the security group.</param>
        public async Task RemoveSecurityGroupUsers(Guid SecurityGroupId)
        {
            var result = await _context.SecurityGroupUsers.Where(x => x.SecurityGroupId == SecurityGroupId).ToListAsync();
            if (result.Count > 0)
            {
                _securityGroupUserRepo.RemoveRange(result);
            }
        }

        /// <summary>
        /// Removes all security group users assigned to a specific user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        public async Task RemoveAllSecurityGroupUsersAssignedToUserID(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            var result = await _context.SecurityGroupUsers.Where(x => x.UserId == userId).ToListAsync();
            if (result.Count > 0)
            {
                _securityGroupUserRepo.RemoveRange(result);
            }
        }

        /// <summary>
        /// Retrieves a list of user IDs assigned to a specific security group.
        /// </summary>
        /// <param name="securityGroupId">The ID of the security group.</param>
        /// <returns>A list of user IDs.</returns>
        public async Task<List<string>> GetAssignedUserIdsBySecurityGroupId(Guid securityGroupId)
        {
            if (securityGroupId == Guid.Empty)
            {
                throw new ArgumentException("Security group ID cannot be empty.", nameof(securityGroupId));
            }

            var userIds = await _context.SecurityGroupUsers
                                         .Where(x => x.SecurityGroupId == securityGroupId)
                                         .Select(x => x.UserId)
                                         .ToListAsync();

            return userIds;
        }

    }
}
