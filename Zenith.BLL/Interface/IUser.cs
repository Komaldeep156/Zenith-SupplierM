using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IUser
    {
        public List<GetUserListDTO> GetUsers();
        public Task<List<ApplicationUser>> GetReportingManagersAsync();
        public Task<GetUserListDTO> GetUserByIdAsync(string userId);
        public Task<string> AddNewUser(RegisterUserModel model, IUrlHelper Url, string requestScheme);
        public Task<bool> UpdateUser(RegisterUserModel model);
        public Task<bool> UpdateUserActiveInactive(string userId, bool isActive);
        int AddContact(ContactDTO contact);
        int AddFile(AttachmentDTO File);
        public ApplicationUser GetUserByEmail(string emailId);
    }
}
