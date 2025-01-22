using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
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
        private readonly IRepository<SecurityGroupFields> _securityGroupFields;
        private readonly ISecurityGroupUsersLogic _securityGroupUsersLogic;

        public SecurityGroupLogic(ZenithDbContext context,
            IRepository<SecurityGroups> securityGroupRepo,
            IRepository<SecurityGroupFields> securityGroupFields,
            ISecurityGroupUsersLogic securityGroupUsersLogic)
        {
            _context = context;
            _securityGroupRepo = securityGroupRepo;
            _securityGroupFields = securityGroupFields;
            _securityGroupUsersLogic = securityGroupUsersLogic;
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
        public async Task<List<SecurityGroupsDTO>> GetAllSecurityGroups(Guid? securityGroupId = null, string fieldName = null, string searchText = null)
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
                    command.Parameters.AddWithValue("@SECURITYGROUPID", securityGroupId);
                }

                if (!string.IsNullOrEmpty(fieldName))
                {
                    command.Parameters.AddWithValue("@fieldName", fieldName);
                }

                if (!string.IsNullOrEmpty(searchText))
                {
                    command.Parameters.AddWithValue("@SearchText", searchText);
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

        public async Task<SecurityGroupsDTO> GetSecurityGroupsByIdOLD(Guid? securityGroupId = null)
        {
            try
            {
                if (!securityGroupId.HasValue || securityGroupId == Guid.Empty)
                {
                    throw new ArgumentException("Security Group ID must be provided and cannot be empty.");
                }

                var connectionstring = _context.Database.GetConnectionString();
                await using var connection = new SqlConnection(connectionstring);
                await connection.OpenAsync();

                // Call stored procedure
                await using var command = new SqlCommand("GETSECURITYGROUPDETAILS", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@SECURITYGROUPID", securityGroupId.Value);

                await using var reader = await command.ExecuteReaderAsync();
                var securityGroupList = new List<SecurityGroupsDTO>();

                // Read data from the stored procedure
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

                if (!securityGroupList.Any())
                {
                    return null;
                }

                var model = securityGroupList.FirstOrDefault();

                var securityGroupFields = await (from f in _context.Fields
                                                 join sgf in _context.SecurityGroupFields
                                                 on f.Id equals sgf.FieldId
                                                 where sgf.SecurityGroupId == securityGroupId
                                                 orderby f.WindowName
                                                 select new FieldsDTO
                                                 {
                                                     Id = sgf.Id,
                                                     WindowName = f.WindowName,
                                                     SectionName = f.SectionName,
                                                     TabName = f.TabName,
                                                     FieldCode = f.FieldName,
                                                     FieldName = f.FieldName,
                                                     FieldId = sgf.FieldId,
                                                     IsView = sgf.IsView,
                                                     IsEdit = sgf.IsEdit,
                                                     IsDelete = sgf.IsDelete
                                                 }).ToListAsync();

                model.Fields = securityGroupFields;

                var users = await (from SGU in _context.SecurityGroupUsers
                                   join U in _context.Users
                                   on SGU.UserId equals U.Id
                                   where SGU.SecurityGroupId == securityGroupId
                                   select new AssignedUser
                                   {
                                       UserId = Guid.Parse(U.Id),
                                       UserName = U.FullName
                                   }).ToListAsync();

                model.AssignedUsers = users;

                return model;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while executing the database query.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving security group details.", ex);
            }
        }

        /// <summary>
        /// Get record by ID 
        /// </summary>
        /// <param name="securityGroupId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<SecurityGroupsDTO> GetSecurityGroupsById(Guid? securityGroupId = null)
        {
            try
            {
                if (!securityGroupId.HasValue || securityGroupId == Guid.Empty)
                {
                    throw new ArgumentException("Security Group ID must be provided and cannot be empty.");
                }

                var connectionstring = _context.Database.GetConnectionString();
                await using var connection = new SqlConnection(connectionstring);
                await connection.OpenAsync();

                await using var command = new SqlCommand("GETSECURITYGROUPDETAILS", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@SECURITYGROUPID", securityGroupId.Value);

                await using var reader = await command.ExecuteReaderAsync();

                var securityGroup = new SecurityGroupsDTO();

                // Read Security Group details
                if (await reader.ReadAsync())
                {
                    securityGroup = new SecurityGroupsDTO
                    {
                        Id = GetValueOrDefault<Guid>(reader, "Id"),
                        Name = GetValueOrDefault<string>(reader, "Name"),
                        SecurityGroupCode = GetValueOrDefault<string>(reader, "SecurityGroupCode"),
                        Description = GetValueOrDefault<string>(reader, "Description"),
                        IsActive = GetValueOrDefault<bool>(reader, "IsActive"),
                        CreatedBy = GetValueOrDefault<string>(reader, "CreatedBy"),
                        CreatedByName = GetValueOrDefault<string>(reader, "CREATEDBYNAME"),
                        CreatedOn = GetValueOrDefault<DateTime>(reader, "CreatedOn"),
                        ModifiedOn = GetValueOrDefault<DateTime>(reader, "ModifiedOn"),
                        ModifiedBy = GetValueOrDefault<string>(reader, "ModifiedBy"),
                        ModifiedByName = GetValueOrDefault<string>(reader, "MODIFIEDBYNAME")
                    };
                }

                // Read Security Group fields
                if (await reader.NextResultAsync())
                {
                    securityGroup.Fields = new List<FieldsDTO>();
                    while (await reader.ReadAsync())
                    {
                        securityGroup.Fields.Add(new FieldsDTO
                        {
                            Id = GetValueOrDefault<Guid>(reader, "Id"),
                            WindowName = GetValueOrDefault<string>(reader, "WindowName"),
                            SectionName = GetValueOrDefault<string>(reader, "SectionName"),
                            TabName = GetValueOrDefault<string>(reader, "TabName"),
                            FieldCode = GetValueOrDefault<string>(reader, "FieldCode"),
                            FieldName = GetValueOrDefault<string>(reader, "FieldName"),
                            FieldId = GetValueOrDefault<Guid>(reader, "FieldId"),
                            IsView = GetValueOrDefault<bool>(reader, "IsView"),
                            IsEdit = GetValueOrDefault<bool>(reader, "IsEdit"),
                            IsDelete = GetValueOrDefault<bool>(reader, "IsDelete")
                        });
                    }
                }

                // Read Security Group users
                if (await reader.NextResultAsync())
                {
                    securityGroup.AssignedUsers = new List<AssignedUser>();
                    while (await reader.ReadAsync())
                    {
                        securityGroup.AssignedUsers.Add(new AssignedUser
                        {
                            UserId = Guid.Parse(GetValueOrDefault<string>(reader, "UserId")),
                            UserName = GetValueOrDefault<string>(reader, "UserName")
                        });
                    }
                }

                return securityGroup;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while executing the database query.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving security group details.", ex);
            }
        }

        public async Task<int> IsDuplicateSecurityGroup(SecurityGroupsDTO model)
        {
            var existingGroups = await _context.SecurityGroups
                .Where(x => x.Name == model.Name || x.SecurityGroupCode == model.SecurityGroupCode)
                .ToListAsync();

            if (existingGroups.Any(x => x.Name == model.Name && x.SecurityGroupCode == model.SecurityGroupCode))
            {
                return 1;
            }
            else if (existingGroups.Any(x => x.Name == model.Name))
            {
                return 2;
            }
            else if (existingGroups.Any(x => x.SecurityGroupCode == model.SecurityGroupCode))
            {
                return 2;
            }

            return 0;
        }

        public async Task<Guid> AddSecurityGroup(SecurityGroupsDTO model)
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

            if (model.SecurityGroupFieldsDTOList != null && securityGroup.Id != Guid.Empty)
            {
                List<SecurityGroupFields> securityGroupFieldsInsertObj = new List<SecurityGroupFields>();
                foreach (var item in model.SecurityGroupFieldsDTOList)
                {
                    securityGroupFieldsInsertObj.Add(new SecurityGroupFields
                    {
                        FieldId = item.FieldId,
                        SecurityGroupId = securityGroup.Id,
                        IsView = item.IsView,
                        IsEdit = item.IsEdit,
                        IsDelete = item.IsDelete,
                        CreatedBy = model.CreatedBy,
                        CreatedOn = DateTime.Now
                    });
                }
                _securityGroupFields.AddRange(securityGroupFieldsInsertObj);
            }

            return securityGroup.Id;
        }

        public async Task<(bool isSuccess, string message)> UpdateSecurityGroup(SecurityGroupsDTO model)
        {
            try
            {
                if (model == null) { throw new ArgumentNullException("model"); }

                var securityGroup = await _securityGroupRepo.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

                if (!model.IsActive)
                {
                    if (await IsAnyUserMappedToSecurityGroup(model.Id))
                    {
                        return (false, "Cannot in active. A reference to this security group is present. ");
                    }
                }

                if (securityGroup != null)
                {
                    securityGroup.Name = model.Name;
                    securityGroup.SecurityGroupCode = model.SecurityGroupCode;
                    securityGroup.Description = model.Description;
                    securityGroup.IsActive = model.IsActive;
                    securityGroup.ModifiedBy = model.CreatedBy;
                    securityGroup.ModifiedOn = DateTime.Now;

                    _securityGroupRepo.Update(securityGroup);
                }

                var securityGroupFields = await _securityGroupFields.Where(x => x.SecurityGroupId == securityGroup.Id).ToListAsync();
                if (securityGroupFields.Any())
                {
                    _securityGroupFields.RemoveRange(securityGroupFields);
                }

                if (model.SecurityGroupFieldsDTOList != null && securityGroup.Id != Guid.Empty)
                {
                    List<SecurityGroupFields> securityGroupFieldsInsertObj = new List<SecurityGroupFields>();
                    foreach (var item in model.SecurityGroupFieldsDTOList)
                    {
                        securityGroupFieldsInsertObj.Add(new SecurityGroupFields
                        {
                            FieldId = item.FieldId,
                            SecurityGroupId = securityGroup.Id,
                            IsView = item.IsView,
                            IsEdit = item.IsEdit,
                            IsDelete = item.IsDelete,
                            CreatedBy = model.CreatedBy,
                            CreatedOn = DateTime.Now
                        });
                    }
                    _securityGroupFields.AddRange(securityGroupFieldsInsertObj);
                }

                return (true, "Security group is updated successfully.");
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("model");
            }
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
                            var securityGroupFields = await _securityGroupFields.Where(x => x.SecurityGroupId == securityGroupId).ToListAsync();
                            if (securityGroupFields.Any())
                            {
                                _securityGroupFields.RemoveRange(securityGroupFields);
                            }

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

        public async Task<(bool isSuccess, List<string> notCopySecurityGroupNames)> CopySecurityGroup(List<Guid> securityGroupIds, string loginUserId)
        {
            List<string> notCopySecurityGroupNames = new List<string>();
            bool isSuccess = true;

            if (securityGroupIds != null && securityGroupIds.Any())
            {
                foreach (var securityGroupId in securityGroupIds)
                {
                    var dbSecurityGroup = await _context.SecurityGroups.FirstOrDefaultAsync(x => x.Id == securityGroupId);
                    if (dbSecurityGroup != null)
                    {
                        using (var transaction = await _context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                var copiedSecurityGroup = new SecurityGroups
                                {
                                    Name = dbSecurityGroup.Name + " - Copy",
                                    SecurityGroupCode = dbSecurityGroup.SecurityGroupCode,
                                    Description = dbSecurityGroup.Description,
                                    IsActive = dbSecurityGroup.IsActive,
                                    CreatedBy = loginUserId,
                                    CreatedOn = DateTime.Now,
                                };

                                _context.SecurityGroups.Add(copiedSecurityGroup);
                                await _context.SaveChangesAsync();

                                var assignedFields = await GetSecurityGroupFieldsIdBySecurityGroupId(securityGroupId);
                                if (assignedFields != null && assignedFields.Any())
                                {

                                    List<SecurityGroupFields> securityGroupFieldsInsertObj = new List<SecurityGroupFields>();
                                    foreach (var fieldId in assignedFields)
                                    {
                                        securityGroupFieldsInsertObj.Add(new SecurityGroupFields
                                        {
                                            FieldId = fieldId.FieldId,
                                            SecurityGroupId = copiedSecurityGroup.Id,
                                            IsView = fieldId.IsView,
                                            IsEdit = fieldId.IsEdit,
                                            IsDelete = fieldId.IsDelete,
                                            CreatedBy = loginUserId,
                                            CreatedOn = DateTime.Now
                                        });
                                    }
                                    _securityGroupFields.AddRange(securityGroupFieldsInsertObj);

                                }

                                var assignedUserIds = await _securityGroupUsersLogic.GetAssignedUserIdsBySecurityGroupId(securityGroupId);
                                foreach (var userId in assignedUserIds)
                                {
                                    var securityGroupUsers = new SecurityGroupUsersDTO
                                    {
                                        UserId = userId,
                                        SecurityGroupId = copiedSecurityGroup.Id,
                                        CreatedBy = loginUserId,
                                        CreatedOn = DateTime.Now,
                                    };
                                    await _securityGroupUsersLogic.AddSecurityGroupUsers(securityGroupUsers);
                                }

                                await _context.SaveChangesAsync();
                                await transaction.CommitAsync();
                            }
                            catch (Exception)
                            {
                                _context.Entry(dbSecurityGroup).State = EntityState.Unchanged;
                                notCopySecurityGroupNames.Add(dbSecurityGroup.Name);

                                await transaction.RollbackAsync();
                            }
                        }
                    }
                }

                if (notCopySecurityGroupNames.Count() == securityGroupIds.Count())
                    isSuccess = false;
            }

            return (isSuccess, notCopySecurityGroupNames);
        }

        public async Task<List<SecurityGroupFields>> GetSecurityGroupFieldsIdBySecurityGroupId(Guid securityGroupId)
        {
            if (securityGroupId == Guid.Empty)
            {
                throw new ArgumentException("Security group ID cannot be empty.", nameof(securityGroupId));
            }

            var fields = await _context.SecurityGroupFields
                                        .Where(x => x.SecurityGroupId == securityGroupId)
                                        .ToListAsync();

            return fields;
        }

        public async Task<bool> IsAnyUserMappedToSecurityGroup(Guid securityGroupId)
        {
            if (securityGroupId == Guid.Empty)
            {
                throw new ArgumentException("Security group ID cannot be empty.", nameof(securityGroupId));
            }

            return await _context.SecurityGroupUsers
                .AnyAsync(x => x.SecurityGroupId == securityGroupId);
        }

        public async Task<(bool isSuccess, List<string> notUpdatedSecurityGroupNames)> UpdateSecurityGroupToActive(List<Guid> securityGroupIds, bool IsActive)
        {
            List<string> notUpdatedSecurityGroupNames = new List<string>();
            bool isSuccess = true;

            if (securityGroupIds != null)
            {
                foreach (var securityGroupId in securityGroupIds)
                {
                    var dbSecurityGroup = await _context.SecurityGroups.FirstOrDefaultAsync(x => x.Id == securityGroupId);

                    if (!IsActive)
                    {
                        if (await IsAnyUserMappedToSecurityGroup(securityGroupId))
                        {
                            notUpdatedSecurityGroupNames.Add(dbSecurityGroup.Name);
                            continue;
                        }
                    }

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

        public async Task<List<AssignedSecurityGroups>> GetSecurityGroupsAssignedToUser(string userId)
        {
            try
            {
                var securityGroupList = await (from SGU in _context.SecurityGroupUsers
                                               join SG in _context.SecurityGroups
                                               on SGU.SecurityGroupId equals SG.Id
                                               where SGU.UserId == userId
                                               select new AssignedSecurityGroups
                                               {
                                                   SecurityGroupId = SG.Id,
                                                   SecurityName = SG.Name
                                               }).ToListAsync();
                return securityGroupList;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving security group details.", ex);
            }
        }

    }
}
