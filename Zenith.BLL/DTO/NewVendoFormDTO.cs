using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class NewVendoFormDTO
    {
        public int SNo { get; set; }
        public string RequestType { get; set; }
        public string RequestedByContactEmail { get; set; }
        public string RequiredBy { get; set; }
        public string Priority { get; set; }
        public string SupplierName { get; set; }
        public string SupplierType { get; set; }
        public string SupplierCountry { get; set; }
        public string Scope { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string BusinessCard { get; set; }
        public string WebSite { get; set; }
        public string ErrorMessage { get; set; }
        public Guid SupplierCountryId { get; set; }
        

    }
}
