using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IDropdownList
    {
        public List<GetDropdownListDTO> GetDropdownList();
        public GetDropdownListDTO GetDropdownByName(string name);
        public Task<string> AddNewList(DropdownLists model, string loggedInUserId);
        public Task<string> AddValue(DropdownValueDTO model, string loggedInUserId);
    }
}
