using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SampleProject.API.Swagger
{
    public class AddPaginationAndSearchHeaders : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isGetById = context.ApiDescription?.RelativePath?.Contains("{id}", StringComparison.OrdinalIgnoreCase) ?? false;
            if ((context.ApiDescription?.HttpMethod ?? "") == "GET" && !isGetById)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Page-Number",
                    In = ParameterLocation.Header,
                    Description = "The page number for pagination.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
                });

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Page-Size",
                    In = ParameterLocation.Header,
                    Description = "The number of items per page for pagination.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
                });

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Sort-Column",
                    In = ParameterLocation.Header,
                    Description = "The column to sort by.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" }
                });

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Sort-Direction",
                    In = ParameterLocation.Header,
                    Description = "The direction to sort by (asc or desc).",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" }
                });

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Search-Query",
                    In = ParameterLocation.Header,
                    Description = "The search query parameters as a JSON object.",
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" }
                });
            }
        }
    }

}
