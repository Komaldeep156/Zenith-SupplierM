using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface ISecurityGroup
    {
        /// <summary>
        /// Retrieves a list of security groups from the database, optionally filtering by security group ID, field name, and search text.
        /// The data is fetched by executing the stored procedure 'GETSECURITYGROUPDETAILS' with the provided parameters.
        /// </summary>
        /// <param name="securityGroupId">Optional parameter to filter by security group ID.</param>
        /// <param name="fieldName">Optional parameter to filter by field name.</param>
        /// <param name="searchText">Optional parameter to filter by search text.</param>
        /// <returns>A list of SecurityGroupsDTO objects containing the security group details.</returns>
        /// <exception cref="Exception">Throws an exception if there is an error while executing the stored procedure or reading data.</exception>
        Task<List<SecurityGroupsDTO>> GetAllSecurityGroups(Guid? securityGroupId = null, string name = null, string securityGroupCode = null);
        
        /// <summary>
        /// Retrieves the details of a specific security group from the database by its ID, including the associated fields and users.
        /// It fetches data by executing the stored procedure 'GETSECURITYGROUPDETAILS' and returns the result as a SecurityGroupsDTO object.
        /// </summary>
        /// <param name="securityGroupId">The ID of the security group to retrieve.</param>
        /// <returns>A SecurityGroupsDTO object containing the security group details, including fields and assigned users.</returns>
        /// <exception cref="ArgumentException">Thrown if the security group ID is not provided or is empty.</exception>
        /// <exception cref="SqlException">Thrown if an error occurs while executing the database query.</exception>
        /// <exception cref="Exception">Thrown for unexpected errors during the retrieval process.</exception>
        Task<SecurityGroupsDTO> GetSecurityGroupsById(Guid? securityGroupId = null);

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
        Task<int> IsDuplicateSecurityGroup(SecurityGroupsDTO model);

        /// <summary>
        /// Adds a new security group to the database along with its associated fields (if any).
        /// </summary>
        /// <param name="model">The security group data to be added.</param>
        /// <returns>
        /// Returns the ID of the newly added security group.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided model is null.</exception>
        Task<Guid> AddSecurityGroup(SecurityGroupsDTO model);

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
        Task<(bool isSuccess, string message)> UpdateSecurityGroup(SecurityGroupsDTO model);

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
        Task<(bool isSuccess, List<string> notDeletedSecurityGroupNames)> DeleteSecurityGroup(List<Guid> securityGroupIds);

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
        Task<(bool isSuccess, List<string> notCopySecurityGroupNames, List<Guid> copiedSecurityGroupId)> CopySecurityGroup(List<Guid> securityGroupIds, string loginUserId);

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
        Task<List<SecurityGroupFields>> GetSecurityGroupFieldsIdBySecurityGroupId(Guid securityGroupId);

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
        Task<(bool isSuccess, List<string> notUpdatedSecurityGroupNames)> UpdateSecurityGroupToActive(List<Guid> securityGroupIds, bool IsActive);

        /// <summary>
        /// Retrieves the list of security groups assigned to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose assigned security groups are to be retrieved.</param>
        /// <returns>A list of assigned security groups, containing the security group ID and name.</returns>
        /// <exception cref="Exception">Thrown when an error occurs while retrieving security group details from the database.</exception>
        Task<List<AssignedSecurityGroups>> GetSecurityGroupsAssignedToUser(string userId);
    }
}
