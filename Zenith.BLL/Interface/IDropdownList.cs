﻿using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IDropdownList
    {
        public List<GetDropdownListDTO> GetDropdownList();
        List<GetDropdownValueDTO> GetDropdownListByArry(Array codeArry);
        public GetDropdownListDTO GetDropdownByName(string name);
        public Task<string> AddNewList(DropdownLists model, string loggedInUserId);
        public Task<string> AddValue(DropdownValueDTO model, string loggedInUserId);
        Guid GetIdByDropdownValue(string listName, string value);
        Guid GetIdByDropdownCode(string listName, string code);
        Task<string> GetDropDownValuById(Guid id);
    }
}
