using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DataQI.Commons.Extensions.Reflection
{
    [ExcludeFromCodeCoverage]
    public static class TypeExtensions
    {
        public static bool HasMethod(this Type type, string name)
            => TryGetMethod(type, name, out _);

        public static bool TryGetMethod(this Type type, string name, out MethodInfo method)
        {
            method = type.GetMethod(name);
            return method != null;
        }

        public static MethodInfo[] GetInstancePublicMethods(this Type type)
            => type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
    }
}
