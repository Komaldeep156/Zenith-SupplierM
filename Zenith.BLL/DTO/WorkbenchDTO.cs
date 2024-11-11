using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.DTO
{
    public class WorkbenchDTO
    {
        public string ApprovalType { get; set; }
        public int PendingStausCount { get; set; }
        public int WorkingStausCount { get; set; }
        public int TotalCount { get; set; }
    }
}
