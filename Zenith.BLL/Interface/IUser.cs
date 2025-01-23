using Microsoft.AspNetCore.Mvc;
using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IUser
    {
        /// <summary>
        /// Retrieves a list of users.
        /// </summary>
        /// <returns>A list of user DTOs.</returns>
        public List<GetUserListDTO> GetUsers();

        /// <summary>
        /// Retrieves a list of reporting managers asynchronously.
        /// </summary>
        /// <returns>A list of application users who are reporting managers.</returns>
        public Task<List<ApplicationUser>> GetReportingManagersAsync();

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A user DTO.</returns>
        public Task<GetUserListDTO> GetUserByIdAsync(string userId);

        /// <summary>
        /// Retrieves a user by their email.
        /// </summary>
        /// <param name="emailId">The email ID of the user.</param>
        /// <returns>An application user.</returns>
        public ApplicationUser GetUserByEmail(string emailId);

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="model">The register user model.</param>
        /// <param name="Url">The URL helper.</param>
        /// <param name="requestScheme">The request scheme.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public Task<string> AddNewUser(RegisterUserModel model, IUrlHelper Url, string requestScheme);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="model">The register user model.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public Task<bool> UpdateUser(RegisterUserModel model);

        /// <summary>
        /// Updates the active/inactive status of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="isActive">The active status.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public Task<bool> UpdateUserActiveInactive(string userId, bool isActive);

        /// <summary>
        /// Adds a new contact.
        /// </summary>
        /// <param name="model">The contact DTO.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        int AddContact(ContactDTO contact);

        /// <summary>
        /// Adds a new file.
        /// </summary>
        /// <param name="File">The attachment DTO.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        int AddFile(AttachmentDTO File);

        /// <summary>
        /// Retrieves all users reporting to a specific user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of application users reporting to the specified user.</returns>    
        Task<List<ApplicationUser>> GetAllUsersReportingToThisUser(string userId);

        /// <summary>
        /// Checks if a user can be deleted asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A boolean indicating whether the user can be deleted.</returns>
        Task<bool> CanDeleteUserAsync(string userId);
    }
}
