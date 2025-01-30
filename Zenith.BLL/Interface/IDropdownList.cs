using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IDropdownList
    {
        /// <summary>
        /// Retrieves a list of active dropdown lists with their values.
        /// </summary>
        /// <returns>List of GetDropdownListDTO</returns>
        public List<GetDropdownListDTO> GetDropdownList();
        
        /// <summary>
        /// Retrieves dropdown values based on an array of codes.
        /// </summary>
        /// <param name="codeArray">Array of codes</param>
        /// <returns>List of GetDropdownValueDTO</returns>
        List<GetDropdownValueDTO> GetDropdownListByArray(Array codeArray);

        /// <summary>
        /// Retrieves a dropdown list by its name.
        /// </summary>
        /// <param name="name">Name of the dropdown list</param>
        /// <returns>GetDropdownListDTO</returns>
        public GetDropdownListDTO GetDropdownByName(string name);

        /// <summary>
        /// Adds a new dropdown list.
        /// </summary>
        /// <param name="model">Dropdown list model</param>
        /// <param name="loggedInUserId">ID of the logged-in user</param>
        /// <returns>string</returns>
        public Task<string> AddNewList(DropdownLists model, string loggedInUserId);

        /// <summary>
        /// Adds new values to a dropdown list.
        /// </summary>
        /// <param name="model">Dropdown value model</param>
        /// <param name="loggedInUserId">ID of the logged-in user</param>
        /// <returns>string</returns>
        public Task<string> AddValue(DropdownValueDTO model, string loggedInUserId);

        /// <summary>
        /// Retrieves the ID of a dropdown value based on the list name and value.
        /// </summary>
        /// <param name="listName">Name of the dropdown list</param>
        /// <param name="value">Value of the dropdown item</param>
        /// <returns>Guid</returns>
        Guid GetIdByDropdownValue(string listName, string value);

        /// <summary>
        /// Retrieves the ID of a dropdown value based on the list name and code.
        /// </summary>
        /// <param name="listName">Name of the dropdown list</param>
        /// <param name="code">Code of the dropdown item</param>
        /// <returns>Guid</returns>
        Guid GetIdByDropdownCode(string listName, string code);

        /// <summary>
        /// Retrieves the value of a dropdown by its ID.
        /// </summary>
        /// <param name="id">ID of the dropdown value</param>
        /// <returns>string</returns>
        Task<string> GetDropDownValueById(Guid id);
    }
}
