using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace DataQI.Commons.Extensions.Reflection
{
    [ExcludeFromCodeCoverage]
    public static class MehodInfoExtensions
    {
        public static string UniqueName(this MethodInfo method)
        {
            var key = $"{method.Name}:{method.ReturnType.GetFriendlyName()}";
            var paramIndex = 0;
            key = method
            .GetParameters()
            .Aggregate($"{key}", (currentKey, param) => 
                $"{currentKey}_arg{paramIndex++}:{param.ParameterType.GetFriendlyName()}");
            return key;
        }
    }
}