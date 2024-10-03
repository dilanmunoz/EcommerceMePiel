using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace EcommerceMePiel.Modelos
{

    [AttributeUsage(AttributeTargets.Method)]
    public class CustomHeaderRequiredAttribute : Attribute
    {
    }

    public class AddCustomHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Verifica si el método tiene el atributo CustomHeaderRequired
            if (context.MethodInfo.GetCustomAttributes(typeof(CustomHeaderRequiredAttribute), false).Any())
            {
                operation.Parameters.Add(new OpenApiParameter
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

}
