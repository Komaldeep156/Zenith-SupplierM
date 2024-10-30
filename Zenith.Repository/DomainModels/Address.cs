using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.DomainModels
{
    public class Address: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
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
        public Guid TenantId { get; set; }


        [ForeignKey("TenantId")]
        public virtual Tenants Tenant { get; set; }

        [ForeignKey("LocalTownId")]
        public virtual DropdownValues DropdownValues_LocalTown { get; set; }
        [ForeignKey("DistrictId")]
        public virtual DropdownValues DropdownValues_District { get; set; }
        [ForeignKey("StateId")]
        public virtual DropdownValues DropdownValues_State { get; set; }
        [ForeignKey("CountryId")]
        public virtual DropdownValues DropdownValues_Country { get; set; }

        [ForeignKey("CodeTypeId")]
        public virtual DropdownValues DropdownValues_CodeType { get; set; }
        [ForeignKey("GeorgraphicLocationId")]
        public virtual DropdownValues DropdownValues_GeorgraphicLocation { get; set; }

    }
}