using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EcommerceMePiel.Filtros
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Authorization : Attribute, IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            CustomAttribute Authorization = context.RequireAttribute<Authorization>();

            if (!Authorization.ContainsAttribute)
                return;

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Required = true, // Cambia a true si es obligatorio
                Description = "Token de autorización para acceder a este endpoint.",
                Schema = new OpenApiSchema { Type = "string" }
            });
        }



    }
}
