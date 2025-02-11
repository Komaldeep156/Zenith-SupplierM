﻿using Microsoft.EntityFrameworkCore;
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

        public List<GetDropdownListDTO> GetDropdownList()
        {
            var dropdowns = _dropdownRepository
                             .Where(dropdown => dropdown.IsActive)
                             .Select(dropdown => new GetDropdownListDTO
                             {
                                 Id = dropdown.Id,
                                 Name = dropdown.Name,
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

        public async Task<string> AddNewList(DropdownLists model, string loggedInUserId)
        {
            var alreadyName = await _dropdownRepository.Where(x=> x.Name == model.Name).FirstOrDefaultAsync();

            if(alreadyName == null)
            {
                DropdownLists newList = new DropdownLists()
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = true,
                    CreatedBy = Guid.Parse(loggedInUserId),
                    ModifiedBy = Guid.Parse(loggedInUserId),
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                };
                await _dropdownRepository.InsertAsync(newList);
                return "ok";
            }
            return "Name already exist";
        }
        public async Task<string> AddValue(DropdownValueDTO model, string loggedInUserId)
        {
            if(model.values != null) { 
                foreach(var value in model.values)
                {
                    var alreadyName = await _dropdownvalueRepository.Where(x => x.Value == value).FirstOrDefaultAsync();

                    if (alreadyName == null)
                    {
                        DropdownValues newList = new DropdownValues()
                        {
                            Value = value,
                            Description = model.Description,
                            DropdownParentNameId = model.DropdownParentNameId,
                            IsActive = true,
                            CreatedBy = Guid.Parse(loggedInUserId),
                            ModifiedBy = Guid.Parse(loggedInUserId),
                            CreatedOn = DateTime.UtcNow,
                            ModifiedOn = DateTime.UtcNow,
                        };
                        await _dropdownvalueRepository.InsertAsync(newList);
                    }
                }
               // return "ok";
            }

            return "Name already exist";
        }
    }
}
