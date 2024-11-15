using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class VacationRequestsDTO: VacationRequests
    {
        public bool IsDelgateRequested { get; set; }
    }
}
