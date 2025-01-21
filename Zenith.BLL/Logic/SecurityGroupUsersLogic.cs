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

        public async Task RemoveSecurityGroupUsers(Guid SecurityGroupId)
        {
            var result = await _context.SecurityGroupUsers.Where(x => x.SecurityGroupId == SecurityGroupId).ToListAsync();
            if (result.Count > 0)
            {
                _securityGroupUserRepo.RemoveRange(result);
            }
        }

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
