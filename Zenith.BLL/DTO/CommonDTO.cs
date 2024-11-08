using Microsoft.AspNetCore.Http;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class CommonResponse
    {
        public string Message { get; set; }
        public string StatusCode { get; set; }
    }

    public class ContactDTO
    {
        public string Name { get; set; }
        public int Gender { get; set; }
        public int Phone { get; set; }
        public int Mobile { get; set; }
        public int Whatsapp { get; set; }
        public string Email { get; set; }
        public int Position { get; set; }
        public int ContactProfile { get; set; }
        public int PrimaryContact { get; set; }
    }

    public class AddressDTO
    {
        public string FullAddress { get; set; }
        public int OfficeNo { get; set; }
        public string BuildingName { get; set; }
        public string Street { get; set; }
        public Guid LocalTownId { get; set; }
        public Guid DistrictId { get; set; }
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public int Code { get; set; }
        public Guid CodeTypeId { get; set; }
        public Guid GeorgraphicLocationId { get; set; }
    }

    public class GetUserListDTO : ApplicationUser
    {

    }

    public class AttachmentDTO
    {
        public IFormFile File { get; set; }
    }
}
