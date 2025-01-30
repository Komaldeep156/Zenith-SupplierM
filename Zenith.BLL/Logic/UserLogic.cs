using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Web.Helpers;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;
using Zenith.Repository.Enums;
namespace Zenith.BLL.Logic
{
    public class UserLogic : IUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly EmailUtils _emailUtils;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly ZenithDbContext _zenithDbContext;

        public UserLogic(UserManager<ApplicationUser>  userManager, 
            EmailUtils emailUtils,
            RoleManager<IdentityRole> roleManager,
            ZenithDbContext zenithDbContext)
        { 
         _userManager = userManager;
         _emailUtils = emailUtils;
         _roleManager = roleManager;
         _zenithDbContext = zenithDbContext;
        }

        #region Utilities

        /// <summary>
        /// Generates a password asynchronously.
        /// </summary>
        /// <returns>A generated password string.</returns>
        public string GeneratePasswordAsync()
        {
            string password = "Zen" + "@" + Guid.NewGuid().ToString("N").Substring(0, 7);
            return password;
        }

        /// <summary>
        /// Generates a unique code.
        /// </summary>
        /// <returns>A generated unique code string.</returns>
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

        #endregion


        /// <summary>
        /// Retrieves a list of users.
        /// </summary>
        /// <returns>A list of user DTOs.</returns>
        public List<GetUserListDTO> GetUsers()
        {
            var data = (from a in _userManager.Users
                         join userRole in _zenithDbContext.UserRoles on a.Id equals userRole.UserId
                         join role in _zenithDbContext.Roles on userRole.RoleId equals role.Id
                         select new GetUserListDTO
                        {
                            Id = a.Id,
                            UserCode = a.UserCode,
                            FullName = a.FullName,
                            Email = a.Email,
                            PhoneNumber = a.PhoneNumber,
                            UserName = a.UserName,
                            DropdownValues_Department = a.DropdownValues_Department,
                            Country = a.Country,
                            Branch = a.Branch,
                            ReportingManager = a.ReportingManager,
                            IsActive = a.IsActive,
                            IsVocationModeOn = a.IsVocationModeOn,
                            RoleId= userRole.RoleId,
                            RoleName= role.Name??"",
                         }).ToList();
            return data;
        }

        /// <summary>
        /// Retrieves a list of reporting managers asynchronously.
        /// </summary>
        /// <returns>A list of application users who are reporting managers.</returns>
        public async Task<List<ApplicationUser>> GetReportingManagersAsync()
        {
            var roleNames = new List<string>
            {
                RolesEnum.VENDOR_MANAGER.GetStringValue().ToUpper(),
                RolesEnum.SENIORVP.GetStringValue().ToUpper(),
                RolesEnum.QHSCMANAGER.GetStringValue().ToUpper(),
            };

            // Normalize role names to uppercase
            var normalizedRoleNames = roleNames.Select(role => role.ToUpper()).ToList();

            // Retrieve the roles matching the provided names
            var roles = await _roleManager.Roles
                                          .Where(role => normalizedRoleNames.Contains(role.NormalizedName))
                                          .ToListAsync();

            if (roles.Any())
            {
                var roleIds = roles.Select(r => r.Id).ToList();
                // Get a list of users who belong to any of the specified roles
                var usersInRoles = await (from user in _userManager.Users
                                          join userRole in _zenithDbContext.Set<IdentityUserRole<string>>() on user.Id equals userRole.UserId
                                          where roleIds.Contains(userRole.RoleId)
                                          select user).Distinct().ToListAsync();

                return usersInRoles;
            }

            return new List<ApplicationUser>();
        }

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A user DTO.</returns>
        public async Task<GetUserListDTO> GetUserByIdAsync(string userId)
        {
            var data = (from a in _userManager.Users
                        where a.Id == userId
                        select new GetUserListDTO
                        {
                            Id = a.Id,
                            UserCode = a.UserCode,
                            FullName = a.FullName,
                            Email = a.Email,
                            PhoneNumber = a.PhoneNumber,
                            UserName = a.UserName,
                            DropdownValues_Department = a.DropdownValues_Department,
                            DepartmentId = a.DepartmentId,
                            Country = a.Country,
                            CountryId = a.CountryId,
                            Branch = a.Branch,
                            BranchId = a.BranchId,
                            ReportingManager = a.ReportingManager,
                            ReportingManagerId = a.ReportingManagerId,
                            IsActive = a.IsActive,
                            IsVocationModeOn = a.IsVocationModeOn,
                            
                        }).FirstOrDefault();

            if (data!=null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                var roleNames = await _userManager.GetRolesAsync(user);
                foreach (var roleName in roleNames)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        data.RoleId= role.Id;
                        data.RoleName = role.Name;
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Retrieves a user by their email.
        /// </summary>
        /// <param name="emailId">The email ID of the user.</param>
        /// <returns>An application user.</returns>
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

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="model">The register user model.</param>
        /// <param name="Url">The URL helper.</param>
        /// <param name="requestScheme">The request scheme.</param>
        /// <returns>A string indicating the result of the operation.</returns>
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
                    IsActive = true,
                };
                user.Id = user.Id.ToUpper();
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

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="model">The register user model.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public async Task<bool> UpdateUser(RegisterUserModel model)
        {
            var user = _userManager.Users.Where(x => x.Id == model.userId).FirstOrDefault();

            if(user != null)
            {
                user.Email = model.Username;
                user.ReportingManagerId = model.ReportingManagerId;
                user.BranchId = model.BranchId;
                user.DepartmentId = model.DepartmentId;
                user.CountryId = model.CountryId;
                user.PhoneNumber = model.PhoneNumber;
                user.FullName = model.FullName;
                user.UserName = model.Username;
                user.PhoneNumber = model.PhoneNumber;
                user.IsActive = model.IsActive;
                user.IsVocationModeOn = model.IsVacationModeOn;
                await _userManager.UpdateAsync(user);

                var currentRoles = await _userManager.GetRolesAsync(user);
                var newRole=await _roleManager.FindByIdAsync(model.RoleId);
                var userRole = currentRoles.FirstOrDefault();
                // If the current roles are the same as the requested roles, no update is needed
                if ( newRole!=null)
                {
                    if ( userRole != newRole.Name)
                    {
                        var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        if (removeRolesResult.Succeeded)
                        {
                            var addResult = await _userManager.AddToRolesAsync(user, new string[] { newRole.Name });
                        }
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the active/inactive status of a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="isActive">The active status.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public async Task<bool> UpdateUserActiveInactive(string userId, bool isActive)
        {
            var user = await _userManager.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                user.IsActive = isActive;
                await _userManager.UpdateAsync(user);
                return true;
            }
            return false;
        }      

        /// <summary>
        /// Adds a new contact.
        /// </summary>
        /// <param name="model">The contact DTO.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        public int AddContact(ContactDTO model)
        {
            return 1;
        }

        /// <summary>
        /// Adds a new file.
        /// </summary>
        /// <param name="File">The attachment DTO.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        public int AddFile(AttachmentDTO File)
        {
            return 1;
        }
        
        public async Task<List<ApplicationUser>> GetAllUsersReportingToThisUser(string userId)
        {
            var reportingUsers = await _userManager.Users.Where(x => x.ReportingManagerId == userId).ToListAsync();
            return reportingUsers;
        }

        /// <summary>
        /// Checks if a user can be deleted asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A boolean indicating whether the user can be deleted.</returns>
        public async Task<bool> CanDeleteUserAsync(string userId)
        {
            if (await _zenithDbContext.DropdownLists.AnyAsync(o => o.CreatedBy == userId || o.ModifiedBy == userId))
                return false;
            if (await _zenithDbContext.DropdownValues.AnyAsync(o => o.CreatedBy == userId || o.ModifiedBy == userId))
                return false;
            if (await _zenithDbContext.VendorsInitializationForm.AnyAsync(o => o.CreatedBy == userId || o.ModifiedBy == userId || o.RequestedBy == new Guid(userId)))
                return false;
            if (await _zenithDbContext.VacationRequests.AnyAsync(o => o.CreatedBy == userId || o.ModifiedBy == userId || o.RequestedByUserId == userId))
                return false;

            return true;
        }
    }
}
