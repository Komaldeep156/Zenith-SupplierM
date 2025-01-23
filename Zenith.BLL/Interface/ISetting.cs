namespace Zenith.BLL.Interface
{
    public interface ISetting
    {
        /// <summary>
        /// Changes the password for the logged-in user.
        /// </summary>
        /// <param name="currentPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password to be set.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public Task<string> ChangePassword(string currentPassword, string newPassword, string loggedInUserId);
    }
}
