using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finbuckle.MultiTenant.Abstractions;

namespace SampleProject.Infrastructure.Tenant
{
    public class CustomTenantInfo : ITenantInfo
    {
        public string? Id { get; set; } = default!;
        public string? Identifier { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string ConnectionString { get; set; } = default!;
    }
}
