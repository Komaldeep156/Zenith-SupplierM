using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Sockets;
using System.Text;
using System.Web.Helpers;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
namespace Zenith.BLL.Logic
{
    public class UserLogic : IUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly EmailUtils _emailUtils;
        public readonly RoleManager<IdentityRole> _roleManager;

        public UserLogic(UserManager<ApplicationUser>  userManager, EmailUtils emailUtils, RoleManager<IdentityRole> roleManager)
        { 
         _userManager = userManager;
         _emailUtils = emailUtils;
         _roleManager = roleManager;
        }

        public List<ApplicationUser> GetUsers()
        {
            var data = _userManager.Users.ToList();
            return data;
        }
        public GetUserListDTO GetUserById(string userId)
        {
            var data = (from a in _userManager.Users
                        where a.Id == userId
                        select new GetUserListDTO
                        {
                            Id = a.Id,
                            UserName = a.UserName,
                            NormalizedUserName = a.NormalizedUserName,
                            Email = a.Email,
                            NormalizedEmail = a.NormalizedEmail,
                            PhoneNumber = a.PhoneNumber,
                            IsApproved = a.IsApproved,
                        }).FirstOrDefault();
            return data;
        }
        public async Task<string> AddNewUser(RegisterUserModel model,IUrlHelper Url, string requestScheme, Guid tenantId)
        {
            ApplicationUser userObj = await _userManager.FindByEmailAsync(model.Username);

            if (userObj != null)
            {
                return "User already exists";
            }
            var password = GeneratePasswordAsync();
            var role = _roleManager.FindByIdAsync(model.Role).Result;
            var user = new ApplicationUser { UserName = model.Username, Email = model.Username, PhoneNumber = model.PhoneNumber, TenantId = tenantId };
            var result = await _userManager.CreateAsync(user, password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: requestScheme);
                _emailUtils.AccountConfirmationMail(model.Username, password, callbackUrl);
            }
            return "Registration success. Please check your email to confirm your account.";
        }
        public async Task<string> UpdateUser(RegisterUserModel model)
        {
            var user = _userManager.Users.Where(x => x.Id == model.userId).FirstOrDefault();

            if(user != null)
            {
                user.UserName = model.Username;
                user.PhoneNumber = model.PhoneNumber;
                await _userManager.UpdateAsync(user);
                return "ok";
            }

            return "Something went wrong";
        }
        public int AddContact(ContactDTO model)
        {
            return 1;
        }
        public int AddFile(AttachmentDTO File)
        {
            return 1;
        }
        public string GeneratePasswordAsync()
        {
            string password = "Zen" + "@" + Guid.NewGuid().ToString("N").Substring(0, 7);
            return password;
        }
    }
}
