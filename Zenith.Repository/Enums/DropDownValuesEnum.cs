using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.Enums
{
    public enum DropDownValuesEnum
    {
        [StringValue("CREATED")]
        CREATED,
        [StringValue("PENDING")]
        PENDING,
        [StringValue("CANCELLED")]
        CANCELLED,
        [StringValue("ASSIGNED")]
        ASSIGNED,
        [StringValue("APPROVED")]
        APPROVED,
        [StringValue("REJECTED")]
        REJECTED,
        [StringValue("URGENT")]
        URGENT,
        [StringValue("NORMAL")]
        NORMAL,
        [StringValue("WORKING")]
        WORKING,
        [StringValue("COMPLETED")]
        COMPLETED,
        [StringValue("Delegate Requested")]
        DelegateRequested,
        [StringValue("DELEGATED")]
        DELEGATED,
        [StringValue("ACCEPTED")]
        ACCEPTED,
        [StringValue("DECLINED")]
        DECLINED
    }
}
