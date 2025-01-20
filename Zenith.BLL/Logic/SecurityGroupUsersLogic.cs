using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Logic
{
    public class SecurityGroupUsersLogic : ISecurityGroupUsersLogic
    {
        private readonly ZenithDbContext _context;

        public SecurityGroupUsersLogic(ZenithDbContext context)
        {
            _context = context;
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
