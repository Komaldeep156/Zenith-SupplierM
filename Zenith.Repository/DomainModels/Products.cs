using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.DomainModels
{
    public class Products: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductNameId { get; set; }
        public string ProductFamilyCode { get; set; }
        public Guid ProductFamilyNameId { get; set; }
        public string ProductNameCode { get; set; }
        public Guid BrandId { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brands Brand { get; set; }

        [ForeignKey("ProductNameId")]
        public virtual DropdownValues DropdownValues_ProductName { get; set; }
        [ForeignKey("ProductFamilyNameId")]
        public virtual DropdownValues DropdownValues_ProductFamilyName { get; set; }
    }
}
