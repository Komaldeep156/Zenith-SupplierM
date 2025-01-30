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

        #region Utilities

        /// <summary>
        /// Retrieves the value from a data reader for the specified column name and converts it to the specified type. 
        /// If the value is DBNull, the default value for the type is returned. 
        /// Special handling is provided for converting integer values to boolean values (0 to false, non-zero to true).
        /// </summary>
        /// <typeparam name="T">The type to which the column value should be converted.</typeparam>
        /// <param name="reader">The IDataReader instance to retrieve the value from.</param>
        /// <param name="columnName">The name of the column whose value is to be retrieved.</param>
        /// <returns>The value of the column converted to the specified type, or the default value if the column is DBNull.</returns>
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

        /// <summary>
        /// Checks if any user is assigned to the specified security group.
        /// </summary>
        /// <param name="securityGroupId">The ID of the security group to check for assigned users.</param>
        /// <returns>
        /// A boolean indicating whether any user is mapped to the given security group.
        /// </returns>
        /// <remarks>
        /// This method validates the provided `securityGroupId`, checks the `SecurityGroupUsers` table 
        /// for any user assignments, and returns `true` if at least one user is mapped to the security group.
        /// </remarks>
        public async Task<bool> IsAnyUserMappedToSecurityGroup(Guid securityGroupId)
        {
            if (securityGroupId == Guid.Empty)
            {
                throw new ArgumentException("Security group ID cannot be empty.", nameof(securityGroupId));
            }

            return await _context.SecurityGroupUsers
                .AnyAsync(x => x.SecurityGroupId == securityGroupId);
        }

        #endregion

        /// <summary>
        /// Retrieves a list of security groups from the database, optionally filtering by security group ID, field name, and search text.
        /// The data is fetched by executing the stored procedure 'GETSECURITYGROUPDETAILS' with the provided parameters.
        /// </summary>
        /// <param name="securityGroupId">Optional parameter to filter by security group ID.</param>
        /// <param name="fieldName">Optional parameter to filter by field name.</param>
        /// <param name="searchText">Optional parameter to filter by search text.</param>
        /// <returns>A list of SecurityGroupsDTO objects containing the security group details.</returns>
        /// <exception cref="Exception">Throws an exception if there is an error while executing the stored procedure or reading data.</exception>
        public async Task<List<SecurityGroupsDTO>> GetAllSecurityGroups(Guid? securityGroupId = null, string fieldName = null, string searchText = null)
        {
            var securityGroupList = new List<SecurityGroupsDTO>();
            try
            {
                var connectionString = _context.Database.GetConnectionString();
                await using var connection = new SqlConnection(connectionString);
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

        /// <summary>
        /// Retrieves the details of a specific security group from the database by its ID, including the associated fields and users.
        /// It fetches data by executing the stored procedure 'GETSECURITYGROUPDETAILS' and returns the result as a SecurityGroupsDTO object.
        /// </summary>
        /// <param name="securityGroupId">The ID of the security group to retrieve.</param>
        /// <returns>A SecurityGroupsDTO object containing the security group details, including fields and assigned users.</returns>
        /// <exception cref="ArgumentException">Thrown if the security group ID is not provided or is empty.</exception>
        /// <exception cref="SqlException">Thrown if an error occurs while executing the database query.</exception>
        /// <exception cref="Exception">Thrown for unexpected errors during the retrieval process.</exception>
        public async Task<SecurityGroupsDTO> GetSecurityGroupsById(Guid? securityGroupId = null)
        {
            try
            {
                if (!securityGroupId.HasValue || securityGroupId == Guid.Empty)
                {
                    throw new ArgumentException("Security Group ID must be provided and cannot be empty.");
                }

                var connectionString = _context.Database.GetConnectionString();
                await using var connection = new SqlConnection(connectionString);
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

        /// <summary>
        /// Checks if a security group with the same name or security group code already exists in the database.
        /// It compares the name and security group code of the provided model against the existing entries in the database.
        /// </summary>
        /// <param name="model">The security group data to check for duplicates.</param>
        /// <returns>
        /// Returns:
        /// 0 - No duplicate found.
        /// 1 - Duplicate found for both name and security group code.
        /// 2 - Duplicate found for either the name or the security group code.
        /// </returns>
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

        /// <summary>
        /// Adds a new security group to the database along with its associated fields (if any).
        /// </summary>
        /// <param name="model">The security group data to be added.</param>
        /// <returns>
        /// Returns the ID of the newly added security group.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided model is null.</exception>
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

        /// <summary>
        /// Updates an existing security group along with its associated fields.
        /// </summary>
        /// <param name="model">The security group data to be updated.</param>
        /// <returns>
        /// A tuple containing a boolean indicating success or failure, and a message with the result.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided model is null.</exception>
        /// <remarks>
        /// If the security group is being deactivated and has associated users, the operation will be rejected.
        /// The method also updates the fields related to the security group.
        /// </remarks>
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

        /// <summary>
        /// Deletes security groups based on the provided list of IDs. If deletion is unsuccessful for any group, 
        /// the group names are returned in the result.
        /// </summary>
        /// <param name="securityGroupIds">The list of security group IDs to delete.</param>
        /// <returns>
        /// A tuple containing a boolean indicating success or failure and a list of names of security groups 
        /// that could not be deleted.
        /// </returns>
        /// <remarks>
        /// The method attempts to delete each security group, including its related fields, from the database.
        /// If deletion fails for any group, the group's name is added to the list of names not deleted.
        /// </remarks>
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

        /// <summary>
        /// Copies security groups based on the provided list of IDs. It creates new security groups with a " - Copy" suffix
        /// in the name and copies related fields and users. If the copying process fails for any group, 
        /// the group names are returned in the result.
        /// </summary>
        /// <param name="securityGroupIds">The list of security group IDs to copy.</param>
        /// <param name="loginUserId">The ID of the user performing the copy operation.</param>
        /// <returns>
        /// A tuple containing a boolean indicating success or failure and a list of names of security groups 
        /// that could not be copied.
        /// </returns>
        /// <remarks>
        /// The method attempts to copy each security group, including its related fields and users, from the database.
        /// If the copying fails for any group, the group's name is added to the list of names not copied.
        /// </remarks>
        public async Task<(bool isSuccess, List<string> notCopySecurityGroupNames, List<Guid> copiedSecurityGroupId)> CopySecurityGroup(List<Guid> securityGroupIds, string loginUserId)
        {
            List<string> notCopySecurityGroupNames = new List<string>();
            List<Guid> copiedSecurityGroupId = new List<Guid>();
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

                                copiedSecurityGroupId.Add(copiedSecurityGroup.Id);

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

            return (isSuccess, notCopySecurityGroupNames, copiedSecurityGroupId);
        }

        /// <summary>
        /// Retrieves the list of fields associated with a specific security group based on its ID.
        /// </summary>
        /// <param name="securityGroupId">The ID of the security group whose fields are to be retrieved.</param>
        /// <returns>
        /// A list of `SecurityGroupFields` objects associated with the provided security group ID.
        /// </returns>
        /// <remarks>
        /// This method checks if the provided `securityGroupId` is not empty, queries the database to fetch 
        /// all the associated fields, and returns the list of fields for the given security group.
        /// </remarks>
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

        /// <summary>
        /// Updates the activation status of security groups. 
        /// It also ensures that security groups with assigned users cannot be deactivated.
        /// </summary>
        /// <param name="securityGroupIds">The list of security group IDs to update.</param>
        /// <param name="IsActive">The new status for the security groups (true for active, false for inactive).</param>
        /// <returns>
        /// A tuple containing:
        /// - `isSuccess`: A boolean indicating whether all security groups were successfully updated.
        /// - `notUpdatedSecurityGroupNames`: A list of names of security groups that were not updated.
        /// </returns>
        /// <remarks>
        /// This method updates the "IsActive" property of security groups. If trying to deactivate a security group
        /// that has assigned users, the update will be skipped, and the group will be added to the "notUpdatedSecurityGroupNames" list.
        /// </remarks>
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

        /// <summary>
        /// Retrieves the list of security groups assigned to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose assigned security groups are to be retrieved.</param>
        /// <returns>A list of assigned security groups, containing the security group ID and name.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while retrieving security group details from the database.</exception>
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
