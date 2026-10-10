using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace DataQI.Commons.Extensions.Reflection
{
    /// <summary>Provides reflection helpers for <see cref="MethodInfo"/>.</summary>
    [ExcludeFromCodeCoverage]
    public static class MehodInfoExtensions
    {
        /// <summary>Builds a key that identifies a method by name, return type and parameter types.</summary>
        /// <param name="method">The method to identify.</param>
        /// <returns>A string such as <c>Find:IEnumerable&lt;Person&gt;_arg0:Func&lt;ICriteria,ICriteria&gt;</c>.</returns>
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