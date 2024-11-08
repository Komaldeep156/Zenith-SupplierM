using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Web.Helpers;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
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
            var data = (from a in _userManager.Users
                        select new ApplicationUser
                        {
                            Id = a.Id,
                            UserCode = a.UserCode,
                            FullName = a.FullName,
                            DropdownValues_Department = a.DropdownValues_Department,
                            Email = a.Email,
                            PhoneNumber = a.PhoneNumber,
                            IsActive = a.IsActive
                        }).ToList();
            return data;
        }

        public async Task<List<ApplicationUser>> GetReportingManagersAsync()
        {
            var roleName = RolesEnum.REPORTING_MANAGER.ToString().Replace("_", " ").ToUpper(); // Converts to "REPORTING MANAGER"

            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(x => x.NormalizedName == roleName);

            if (role != null)
            {
                // Get a list of users who have the "Reporting Manager" role
                var usersInRole = new List<ApplicationUser>();
                var allUsers = await _userManager.Users.ToListAsync();

                foreach (var user in allUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        usersInRole.Add(user);
                    }
                }

                return usersInRole;
            }

            return new List<ApplicationUser>();
        }

        public ApplicationUser GetUserById(string userId)
        {
            var data = (from a in _userManager.Users
                        where a.Id == userId
                        select new ApplicationUser
                        {
                            Id = a.Id,
                            UserCode = a.UserCode,
                            FullName = a.FullName,
                            Email = a.Email,
                            PhoneNumber = a.PhoneNumber,
                        }).FirstOrDefault();
            return data;
        }

        public ApplicationUser GetUserByEmail(string emailId)
        {
            var data = (from a in _userManager.Users
                        where a.Email == emailId
                        select new ApplicationUser
                        {
                            Id = a.Id,
                            UserCode = a.UserCode,
                            Email = a.Email,
                            FullName = a.FullName,
                            PhoneNumber = a.PhoneNumber,
                        }).FirstOrDefault();
            return data;
        }

        public async Task<string> AddNewUser(RegisterUserModel model, IUrlHelper Url, string requestScheme)
        {

            try
            {

                ApplicationUser userObj = await _userManager.FindByEmailAsync(model.Username);

                if (userObj != null)
                {
                    return "User already exists";
                }
                string uniqueCode = GenerateUniqueCode();
                var password = GeneratePasswordAsync();
                var role = _roleManager.FindByIdAsync(model.RoleId).Result;
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Username,
                    UserCode = uniqueCode,
                    ReportingManagerId = model.ReportingManagerId,
                    BranchId = model.BranchId,
                    DepartmentId = model.DepartmentId,
                    CountryId = model.CountryId,
                    PhoneNumber = model.PhoneNumber,
                    FullName = model.FullName,
                };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
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
            catch (Exception ex)
            {

                throw ex;
            }
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

        public string GenerateUniqueCode()
        {
            string code;
            Random rand = new Random();
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

            code = string.Empty;
            for (int i = 0; i < 7; i++)
            {
                code += saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
            }
            return code;
        }
    }

  

}
