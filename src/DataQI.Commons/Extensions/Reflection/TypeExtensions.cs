using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DataQI.Commons.Extensions.Reflection
{
    /// <summary>Provides reflection helpers for <see cref="Type"/>.</summary>
    [ExcludeFromCodeCoverage]
    public static class TypeExtensions
    {
        /// <summary>Determines whether the type has a public method with the given name.</summary>
        /// <param name="type">The type to inspect.</param>
        /// <param name="name">The method name.</param>
        /// <returns><c>true</c> if such a method exists; otherwise, <c>false</c>.</returns>
        public static bool HasMethod(this Type type, string name)
            => TryGetMethod(type, name, out _);

        /// <summary>Gets the public method with the given name, if any.</summary>
        /// <param name="type">The type to inspect.</param>
        /// <param name="name">The method name.</param>
        /// <param name="method">When this method returns, the method found, or <c>null</c>.</param>
        /// <returns><c>true</c> if the method was found; otherwise, <c>false</c>.</returns>
        public static bool TryGetMethod(this Type type, string name, out MethodInfo method)
        {
            method = type.GetMethod(name);
            return method != null;
        }

        /// <summary>Gets the public instance methods of the type.</summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>The public instance methods, including inherited ones.</returns>
        public static MethodInfo[] GetInstancePublicMethods(this Type type)
            => type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        
        /// <summary>Gets a readable name that spells out generic arguments, for example <c>List&lt;String&gt;</c>.</summary>
        /// <param name="type">The type to name.</param>
        /// <returns>The type name; for generic types the name followed by its arguments in angle brackets, separated by commas without spaces.</returns>
        public static string GetFriendlyName(this Type type)
        {
            if (!type.IsGenericType) return type.Name;
            var genericTypeName = type.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
            var genericArgs = string.Join(",", type.GetGenericArguments().Select(GetFriendlyName));
            return $"{genericTypeName}<{genericArgs}>";
        }

        /// <summary>Gets the type wrapped by a <see cref="System.Threading.Tasks.Task{TResult}"/>.</summary>
        /// <param name="type">The type to inspect.</param>
        /// <param name="resultType">When this method returns, the type argument of <c>Task&lt;T&gt;</c>, or <c>null</c>.</param>
        /// <returns><c>true</c> if <paramref name="type"/> is <c>Task&lt;T&gt;</c>; otherwise, <c>false</c>.</returns>
        public static bool TryGetAsyncResultType(this Type type, out Type resultType)
        {
            resultType = null;

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Task<>))
                return false;

            resultType = type.GetGenericArguments()[0];
            return true;
        }

        /// <summary>Gets the public methods declared by the type and by every interface it inherits.</summary>
        /// <param name="type">The interface type to inspect.</param>
        /// <returns>The methods of the type followed by those of its inherited interfaces.</returns>
        public static MethodInfo[] GetAllInterfaceMethods(this Type type)
            => new[] { type }.Concat(type.GetInterfaces()).SelectMany(i => i.GetMethods()).ToArray();
    }
}
