using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Logic
{
    public class SettingLogic : ISetting
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public SettingLogic(UserManager<ApplicationUser> userManager) 
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Changes the password for the logged-in user.
        /// </summary>
        /// <param name="currentPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password to be set.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public async Task<string> ChangePassword(string currentPassword, string newPassword, string loggedInUserId)
        {
            if (loggedInUserId != null && newPassword != null)
            {
                var user = _userManager.Users.Where(x => x.Id == loggedInUserId).FirstOrDefault();

                if (user != null)
                {

                    var passwordVerificationResult = await _userManager.CheckPasswordAsync(user, currentPassword);

                    if (!passwordVerificationResult)
                    {
                        return "Current password is incorrect";
                    }
                    else
                    {
                        var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                        await _userManager.UpdateAsync(user);
                        return "Password changed successfully!";
                    }
                }
            }
            return "Something went wrong";
        }
    }
}
