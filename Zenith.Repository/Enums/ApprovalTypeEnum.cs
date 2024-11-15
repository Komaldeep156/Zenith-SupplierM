using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.Enums
{
    public enum ApprovalTypeEnum
    {
        [StringValue("VIR")]
        VIR,

        [StringValue("VQR")]
        VQR,

        [StringValue("VACATION")]
        VACATION,
    }
}
