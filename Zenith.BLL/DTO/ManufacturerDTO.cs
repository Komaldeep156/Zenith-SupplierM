using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class ManufacturerDTO
    {
        public string FullName { get; set; }
        public string Website { get; set; }
        public Guid RegisteredSinceId { get; set; }
        public Guid HeadQuarterId { get; set; }
    }

    public class GetManufactureListDTO {

        public Guid Id { get; set; }
        public string ManufacturerCode { get; set; }
        public string ShortName { get; set; }
        public string Website { get; set; }
        public string FullName { get; set; }
        public virtual DropdownValues RegisteredSince { get; set; }
        public virtual DropdownValues HeadQuarter { get; set; }
        public bool IsActive { get; set; }
        public int RevisionNumer { get; set; }
        public string ApprovalStatus { get; set; }
        public string RejectionReason { get; set; }
        //public Guid AddressId { get; set; }
    }

    public class BrandDTO
    {
        public string BrandName { get; set; }
        public IFormFile File { get; set; }
    }

    public class ProductDTO
    {
        public Guid ProductNameId { get; set; }
        public Guid ProductFamilyNameId { get; set; }
    }

}
