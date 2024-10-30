using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.DomainModels
{
    public class Brands:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string BrandCode { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string LogoFilePath { get; set; }
    }
}
