using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SampleProject.API.Swagger
{
    public class AddTenantIdHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Adding the optional X-Tenant-ID header parameter
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Tenant-ID",
                In = ParameterLocation.Header,
                Description = "Optional Tenant ID to be used for multi-tenancy.",
                Required = false, // It is optional
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }

}
