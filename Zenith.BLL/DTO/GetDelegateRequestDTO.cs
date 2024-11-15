using Microsoft.AspNetCore.Http;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class GetDelegateRequestDTO: DelegationRequests
    {
        public DateTime DelegatedRequestedOn { get; set; }
        public string RequestNo { get; set; }
        public string SourceCd { get; set; }

    }
}
