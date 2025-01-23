using Microsoft.EntityFrameworkCore;
using Zenith.BLL.DTO;
using Zenith.BLL.Interface;
using Zenith.Repository.DomainModels;
using Zenith.Repository.RepositoryFiles;

namespace Zenith.BLL.Logic
{
    public class DropdownListLogic : IDropdownList
    {
        private readonly IRepository<DropdownLists> _dropdownRepository;
        private readonly IRepository<DropdownValues> _dropdownvalueRepository;
        public DropdownListLogic(IRepository<DropdownLists> dropdownRepository, 
            IRepository<DropdownValues> dropdownvalueRepository)
        {
            _dropdownRepository = dropdownRepository;
            _dropdownvalueRepository = dropdownvalueRepository;
        }
        
        /// <summary>
        /// Retrieves a list of active dropdown lists with their values.
        /// </summary>
        /// <returns>List of GetDropdownListDTO</returns>
        public List<GetDropdownListDTO> GetDropdownList()
        {
            var dropdowns = _dropdownRepository
                             .Where(dropdown => dropdown.IsActive)
                             .Select(dropdown => new GetDropdownListDTO
                             {
                                 Id = dropdown.Id,
                                 Name = dropdown.Name,
                                 Code = dropdown.Code,
                                 Description = dropdown.Description,
                             })
                             .ToList();

            var dropdownIds = dropdowns.Select(d => d.Id).ToList();
            var dropdownValues = _dropdownvalueRepository
                                 .Where(value => dropdownIds.Contains(value.DropdownParentNameId))
                                 .Select(value => new
                                 {
                                     value.Value,
                                     value.Description,
                                     value.DropdownParentNameId
                                 })
                                 .ToList();

            foreach (var dropdown in dropdowns)
            {
                dropdown.Values = dropdownValues
                                 .Where(v => v.DropdownParentNameId == dropdown.Id)
                                 .Select(v => new GetDropdownValueDTO
                                 {
                                     Value = v.Value,
                                     Description = v.Description,
                                 })
                                 .ToList();
            }

            return dropdowns;
        }

        /// <summary>
        /// Retrieves dropdown values based on an array of codes.
        /// </summary>
        /// <param name="codeArry">Array of codes</param>
        /// <returns>List of GetDropdownValueDTO</returns>
        public List<GetDropdownValueDTO> GetDropdownListByArry(Array codeArry)
        {
            var list = new List<GetDropdownValueDTO>();

            foreach (var code in codeArry)
            {
                var items = _dropdownvalueRepository
                        .Where(v => v.Code == code.ToString()) 
                        .Select(v => new GetDropdownValueDTO
                        {
                            Id = v.Id,
                            Value = v.Value,
                            Description = v.Description,
                        })
                        .ToList();

                list.AddRange(items);
            }

            return list;
        }

        /// <summary>
        /// Retrieves a dropdown list by its name.
        /// </summary>
        /// <param name="name">Name of the dropdown list</param>
        /// <returns>GetDropdownListDTO</returns>
        public GetDropdownListDTO GetDropdownByName(string name)
        {
            var dropdowns = _dropdownRepository
                .Where(dropdown => dropdown.IsActive && dropdown.Name == name)
                .Select(dropdown => new GetDropdownListDTO
                {
                    Id = dropdown.Id,
                    Name = dropdown.Name,
                    Description = dropdown.Description,
                })
                .FirstOrDefault();

            if (dropdowns != null)
            {
                dropdowns.Values = _dropdownvalueRepository
                    .Where(v => v.DropdownParentNameId == dropdowns.Id)
                    .Select(v => new GetDropdownValueDTO
                    {
                        Id = v.Id,
                        Value = v.Value,
                        Description = v.Description,
                    })
                    .ToList();
            }
            else
            {
                return null;
            }

            return dropdowns;
        }

        /// <summary>
        /// Adds a new dropdown list.
        /// </summary>
        /// <param name="model">Dropdown list model</param>
        /// <param name="loggedInUserId">ID of the logged-in user</param>
        /// <returns>string</returns>
        public async Task<string> AddNewList(DropdownLists model, string loggedInUserId)
        {
            var alreadyName = await _dropdownRepository.Where(x => x.Name == model.Name).FirstOrDefaultAsync();

            if (alreadyName == null)
            {
                DropdownLists newList = new DropdownLists()
                {
                    Name = model.Name,
                    Code = model.Code ?? "",
                    Description = model.Description ?? "",
                    IsActive = true,
                    CreatedBy = loggedInUserId,
                    ModifiedBy = loggedInUserId,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                };
                await _dropdownRepository.InsertAsync(newList);
                return "ok";
            }
            return "Name already exist";
        }

        /// <summary>
        /// Adds new values to a dropdown list.
        /// </summary>
        /// <param name="model">Dropdown value model</param>
        /// <param name="loggedInUserId">ID of the logged-in user</param>
        /// <returns>string</returns>
        public async Task<string> AddValue(DropdownValueDTO model, string loggedInUserId)
        {
            if (model.Records != null)
            {
                foreach (var vl in model.Records)
                {
                    var alreadyName = await _dropdownvalueRepository.Where(x => x.Value == vl.Value && x.Code == vl.Code).FirstOrDefaultAsync();

                    if (alreadyName == null && model.DropdownParentNameId != Guid.Empty)
                    {
                        DropdownValues newList = new DropdownValues()
                        {
                            Value = vl.Value ?? "",
                            Code = vl.Code ?? "",
                            Description = model.Description ?? "",
                            DropdownParentNameId = model.DropdownParentNameId,
                            IsActive = true,
                            CreatedBy = loggedInUserId,
                            CreatedOn = DateTime.UtcNow,
                        };
                        await _dropdownvalueRepository.InsertAsync(newList);
                    }
                }
                return "ok";
            }

            return "Name already exist";
        }

        /// <summary>
        /// Retrieves the ID of a dropdown value based on the list name and value.
        /// </summary>
        /// <param name="listName">Name of the dropdown list</param>
        /// <param name="value">Value of the dropdown item</param>
        /// <returns>Guid</returns>
        public Guid GetIdByDropdownValue(string listName, string value)
        {
            Guid returnId;
            var dropdowns = _dropdownRepository
                .Where(dropdown => dropdown.IsActive && dropdown.Name == listName)
                .Select(dropdown => new GetDropdownListDTO
                {
                    Id = dropdown.Id,
                    Name = dropdown.Name,
                    Description = dropdown.Description,
                })
                .FirstOrDefault();

            if (dropdowns != null)
            {
                returnId = _dropdownvalueRepository
                    .Where(v => v.DropdownParentNameId == dropdowns.Id && v.Value == value)
                    .Select(x => x.Id).FirstOrDefault();
            }
            else
            {
                return Guid.Empty;
            }

            return returnId;
        }
        
        /// <summary>
        /// Retrieves the ID of a dropdown value based on the list name and code.
        /// </summary>
        /// <param name="listName">Name of the dropdown list</param>
        /// <param name="code">Code of the dropdown item</param>
        /// <returns>Guid</returns>
        public Guid GetIdByDropdownCode(string listName, string code)
        {
            Guid returnId;
            var dropdowns = _dropdownRepository
                .Where(dropdown => dropdown.IsActive && dropdown.Name == listName)
                .Select(dropdown => new GetDropdownListDTO
                {
                    Id = dropdown.Id,
                    Name = dropdown.Name,
                    Description = dropdown.Description,
                })
                .FirstOrDefault();

            if (dropdowns != null)
            {
                returnId = _dropdownvalueRepository
                    .Where(v => v.DropdownParentNameId == dropdowns.Id && v.Code!=null && v.Code == code)
                    .Select(x => x.Id).FirstOrDefault();
            }
            else
            {
                return Guid.Empty;
            }

            return returnId;
        }

        /// <summary>
        /// Retrieves the value of a dropdown by its ID.
        /// </summary>
        /// <param name="id">ID of the dropdown value</param>
        /// <returns>string</returns>
        public async Task<string> GetDropDownValuById(Guid id)
        {
            var value = await _dropdownvalueRepository.Where(x=>x.Id == id).FirstOrDefaultAsync();

            return value?.Value ?? "";
        }
    }
}
