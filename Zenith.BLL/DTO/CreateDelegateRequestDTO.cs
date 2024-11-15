using Microsoft.AspNetCore.Http;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class CreateDelegateRequestDTO
    {
        public string CommaSprtdRecordIds { get; set; }
        public string RecordTypeCd { get; set; }
        public string DelegateToUserId { get; set; }
    }
}
