using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IUser
    {
        public List<ApplicationUser> GetUsers();
        public GetUserListDTO GetUserById(string userId);
        public Task<string> AddNewUser(RegisterUserModel model, IUrlHelper Url, string requestScheme, Guid tenantId);
        public Task<string> UpdateUser(RegisterUserModel model);
        int AddContact(ContactDTO contact);
        int AddFile(AttachmentDTO File);
    }
}
