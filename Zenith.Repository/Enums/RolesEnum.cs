using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.Enums
{
    public enum RolesEnum
    {
        [StringValue("Admin")]
        ADMIN,

        [StringValue("Vendor Officer")]
        VENDOR_OFFICER,

        [StringValue("Vendor Manager")]
        VENDOR_MANAGER,

        [StringValue("Super Admin")]
        SUPER_ADMIN,

        [StringValue("Manager")]
        MANAGER
    }
}
