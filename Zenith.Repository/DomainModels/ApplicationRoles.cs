﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.DomainModels
{
    public class ApplicationRoles : IdentityRole
    {
        public Guid TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenants Tenant { get; set; }
    }
}
