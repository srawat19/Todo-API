using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Todo.Domain.DTOs;

namespace Todo_API.Swagger.Filters
{
    /// <summary>
    /// PaginatedResponseDTOFilter class helps with describing the properties of PaginatedResponseDTO model
    /// </summary>
    public class PaginatedResponseDTOFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(PaginatedResponseDTO<>))
            {
                schema.Description = "Paginated response format";
                schema.Properties["data"].Description = "Enumerable list of data for the current page";
                schema.Properties["page"].Description = "Current Page number (starting 1)";
                schema.Properties["limit"].Description = "Max records to be displayed per page";
                schema.Properties["totalRecords"].Description = "Total records available in the app";
                schema.Properties["totalPages"].Description = "Total number of pages available";
                schema.Properties["nextPageUrl"].Description = "URL for the next page with records";
                schema.Properties["prevPageUrl"].Description = "URL for the previous records page";




            }
        }
    }
}
