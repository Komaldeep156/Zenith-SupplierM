using System.ComponentModel.DataAnnotations;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class VendorQualificationWorkFlowDTO: VendorQualificationWorkFlow
    {
        public string RoleName { get; set; }
    }

}
