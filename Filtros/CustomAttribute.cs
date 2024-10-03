using Microsoft.AspNetCore.Mvc.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace EcommerceMePiel.Filtros
{
    public class CustomAttribute
    {
        public readonly bool ContainsAttribute;
        public readonly bool Mandatory;

        public CustomAttribute(bool containsAttribute, bool mandatory)
        {
            ContainsAttribute = containsAttribute;
            Mandatory = mandatory;
        }
    }

    public static class OperationFilterContextExtensions
    {
        public static CustomAttribute RequireAttribute<T>(this OperationFilterContext context)
        {
            IEnumerable<IFilterMetadata> globalAttributes = context
                .ApiDescription
                .ActionDescriptor
                .FilterDescriptors
                .Select(p => p.Filter);

            object[] controllerAttributes = context
                .MethodInfo?
                .DeclaringType?
                .GetCustomAttributes(true) ?? Array.Empty<object>();

            object[] methodAttributes = context
                .MethodInfo?
                .GetCustomAttributes(true) ?? Array.Empty<object>();

            List<T> containsHeaderAttributes = globalAttributes
                .Union(controllerAttributes)
                .Union(methodAttributes)
                .OfType<T>()
                .ToList();

            return containsHeaderAttributes.Count == 0
                ? new CustomAttribute(false, false)
                : new CustomAttribute(true, false);
        }
    }
}
