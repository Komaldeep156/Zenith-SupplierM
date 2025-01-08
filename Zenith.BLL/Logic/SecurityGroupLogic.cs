using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Web.Mvc;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class SecurityGroupLogic : ISecurityGroup
    {
        private readonly ZenithDbContext _context;
        private readonly IRepository<SecurityGroups> _securityGroupRepo;

        public SecurityGroupLogic(ZenithDbContext context, IRepository<SecurityGroups> securityGroupRepo)
        {
            _context = context;
            _securityGroupRepo = securityGroupRepo;
        }

        private T GetValueOrDefault<T>(IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return default;

            object value = reader.GetValue(ordinal);

            // Handle special type conversions
            if (typeof(T) == typeof(bool) && value is int intValue)
            {
                return (T)(object)(intValue != 0); // Convert int to bool
            }

            return (T)value; // Default casting
        }
        public async Task<List<SecurityGroupsDTO>> GetAllSecurityGroups(Guid? securityGroupId = null,string fieldName = null, string searchText = null)
        {
            var securityGroupList = new List<SecurityGroupsDTO>();
            try
            {
                var connectionstring = _context.Database.GetConnectionString();
                await using var connection = new SqlConnection(connectionstring);
                await connection.OpenAsync();

                await using var command = new SqlCommand("GETSECURITYGROUPDETAILS", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (securityGroupId.HasValue && securityGroupId != Guid.Empty)
                {
                    command.Parameters.AddWithValue("fieldName", securityGroupId);
                }

                if (!string.IsNullOrEmpty(fieldName))
                {
                    command.Parameters.AddWithValue("fieldName", fieldName);
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    command.Parameters.AddWithValue("SearchText", searchText);
                }

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    securityGroupList.Add(new SecurityGroupsDTO
                    {
                        Id = GetValueOrDefault<Guid>(reader, "id"),
                        Name = GetValueOrDefault<string>(reader, "name"),
                        SecurityGroupCode = GetValueOrDefault<string>(reader, "securitygroupcode"),
                        Description = GetValueOrDefault<string>(reader, "description"),
                        IsActive = GetValueOrDefault<bool>(reader, "isactive"),
                        CreatedBy = GetValueOrDefault<string>(reader, "createdby"),
                        CreatedByName = GetValueOrDefault<string>(reader, "CREATEDBYNAME"),
                        CreatedOn = GetValueOrDefault<DateTime>(reader, "createdon"),
                        ModifiedOn = GetValueOrDefault<DateTime>(reader, "modifiedon"),
                        ModifiedBy = GetValueOrDefault<string>(reader, "modifiedby"),
                        ModifiedByName = GetValueOrDefault<string>(reader, "MODIFIEDBYNAME")
                    });
                }

                return securityGroupList;
            }
            catch
            {
                throw;
            }
        }

        public async Task AddSecurityGroup(SecurityGroupsDTO model)
        {
            if (model == null) { throw new ArgumentNullException("model"); }

            var securityGroup = new SecurityGroups
            {
                Name = model.Name,
                SecurityGroupCode = model.SecurityGroupCode,
                Description = model.Description,
                IsActive = model.IsActive,
                CreatedBy = model.CreatedBy,
                CreatedOn = DateTime.Now
            };

            _securityGroupRepo.Add(securityGroup);
            await Task.CompletedTask;
        }

        public async Task<(bool isSuccess, List<string> notDeletedSecurityGroupNames)> DeleteSecurityGroup(List<Guid> securityGroupIds)
        {
            List<string> notDeletedSecurityGroupNames = new List<string>();
            bool isSuccess = true;

            if (securityGroupIds != null)
            {
                foreach (var securityGroupId in securityGroupIds)
                {
                    var dbSecurityGroup = await _context.SecurityGroups.FirstOrDefaultAsync(x => x.Id == securityGroupId);
                    if (dbSecurityGroup != null)
                    {
                        try
                        {
                            _context.SecurityGroups.Remove(dbSecurityGroup);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            _context.Entry(dbSecurityGroup).State = EntityState.Unchanged;
                            notDeletedSecurityGroupNames.Add(dbSecurityGroup.Name);
                        }
                    }
                }

                if (notDeletedSecurityGroupNames.Count() == securityGroupIds.Count())
                    isSuccess = false;
            }

            return (isSuccess, notDeletedSecurityGroupNames);
        }

        public async Task<(bool isSuccess, List<string> notUpdatedSecurityGroupNames)> UpdateSecurityGroupToActive(List<Guid> securityGroupIds , bool IsActive)
        {
            List<string> notUpdatedSecurityGroupNames = new List<string>();
            bool isSuccess = true;

            if (securityGroupIds != null)
            {
                foreach (var securityGroupId in securityGroupIds)
                {
                    var dbSecurityGroup = await _context.SecurityGroups.FirstOrDefaultAsync(x => x.Id == securityGroupId);
                    if (dbSecurityGroup != null)
                    {
                        try
                        {
                            dbSecurityGroup.IsActive = IsActive;
                            _context.SecurityGroups.Update(dbSecurityGroup);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            _context.Entry(dbSecurityGroup).State = EntityState.Unchanged;
                            notUpdatedSecurityGroupNames.Add(dbSecurityGroup.Name);
                        }
                    }
                }

                if (notUpdatedSecurityGroupNames.Count() == securityGroupIds.Count())
                    isSuccess = false;
            }

            return (isSuccess, notUpdatedSecurityGroupNames);
        }

        //public async Task<SecurityGroupsDTO>GetSecurityGroup(Guid securityGroupId)
        //{
        //    var securityGroup = await _context.SecurityGroups.FirstOrDefaultAsync(x => x.Id == securityGroupId);
        //    if(securityGroup != null)
        //    {
        //        var model = new SecurityGroupsDTO
        //        {
        //            Id = securityGroup.Id,
        //            Name = securityGroup.Name,
        //            SecurityGroupCode = securityGroup.SecurityGroupCode,
        //            Description = securityGroup.Description,
        //            IsActive = securityGroup.IsActive,


        //        };
        //    }
        //    return model;
        //}
    }
}
