using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace DataQI.Commons.Extensions.Reflection
{
    /// <summary>Provides methods to look up custom attributes on reflection elements.</summary>
    [ExcludeFromCodeCoverage]
    public static class CustomAttributeExtensions
    {
        /// <summary>Determines whether the assembly is decorated with an attribute of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <returns><c>true</c> if the attribute is present; otherwise, <c>false</c>.</returns>
        public static bool HasAttribute<T>(this Assembly element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

        /// <summary>Gets the first attribute of type <typeparamref name="T"/> applied to the assembly.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <param name="attribute">When this method returns, the first matching attribute, or <c>null</c>.</param>
        /// <returns><c>true</c> if the attribute is found; otherwise, <c>false</c>.</returns>
        public static bool TryGetAttribute<T>(this Assembly element, out T attribute) where T : Attribute
        {
            attribute = element.GetCustomAttributes()?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        /// <summary>Determines whether the module is decorated with an attribute of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <returns><c>true</c> if the attribute is present; otherwise, <c>false</c>.</returns>
        public static bool HasAttribute<T>(this Module element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

        /// <summary>Gets the first attribute of type <typeparamref name="T"/> applied to the module.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <param name="attribute">When this method returns, the first matching attribute, or <c>null</c>.</param>
        /// <returns><c>true</c> if the attribute is found; otherwise, <c>false</c>.</returns>
        public static bool TryGetAttribute<T>(this Module element, out T attribute) where T : Attribute
        {
            attribute = element.GetCustomAttributes()?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        /// <summary>Determines whether the member is decorated with an attribute of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <returns><c>true</c> if the attribute is present; otherwise, <c>false</c>.</returns>
        public static bool HasAttribute<T>(this MemberInfo element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

        /// <summary>Gets the first attribute of type <typeparamref name="T"/> applied to the member.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <param name="attribute">When this method returns, the first matching attribute, or <c>null</c>.</param>
        /// <returns><c>true</c> if the attribute is found; otherwise, <c>false</c>.</returns>
        public static bool TryGetAttribute<T>(this MemberInfo element, out T attribute) where T : Attribute
        {
            attribute = element.GetCustomAttributes()?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        /// <summary>Determines whether the parameter is decorated with an attribute of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <returns><c>true</c> if the attribute is present; otherwise, <c>false</c>.</returns>
        public static bool HasAttribute<T>(this ParameterInfo element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

        /// <summary>Gets the first attribute of type <typeparamref name="T"/> applied to the parameter.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <param name="attribute">When this method returns, the first matching attribute, or <c>null</c>.</param>
        /// <returns><c>true</c> if the attribute is found; otherwise, <c>false</c>.</returns>
        public static bool TryGetAttribute<T>(this ParameterInfo element, out T attribute) where T : Attribute
        {
            attribute = element.GetCustomAttributes()?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        /// <summary>Determines whether the member is decorated with an attribute of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <param name="inherit"><c>true</c> to also search the inheritance chain of the element.</param>
        /// <returns><c>true</c> if the attribute is present; otherwise, <c>false</c>.</returns>
        public static bool HasAttribute<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute, inherit);
        }

        /// <summary>Gets the first attribute of type <typeparamref name="T"/> applied to the member.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <param name="attribute">When this method returns, the first matching attribute, or <c>null</c>.</param>
        /// <param name="inherit"><c>true</c> to also search the inheritance chain of the element.</param>
        /// <returns><c>true</c> if the attribute is found; otherwise, <c>false</c>.</returns>
        public static bool TryGetAttribute<T>(this MemberInfo element, out T attribute, bool inherit) where T : Attribute
        {
            attribute = element.GetCustomAttributes(inherit)?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        /// <summary>Determines whether the parameter is decorated with an attribute of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <param name="inherit"><c>true</c> to also search the inheritance chain of the element.</param>
        /// <returns><c>true</c> if the attribute is present; otherwise, <c>false</c>.</returns>
        public static bool HasAttribute<T>(this ParameterInfo element, bool inherit) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute, inherit);
        }

        /// <summary>Gets the first attribute of type <typeparamref name="T"/> applied to the parameter.</summary>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <param name="attribute">When this method returns, the first matching attribute, or <c>null</c>.</param>
        /// <param name="inherit"><c>true</c> to also search the inheritance chain of the element.</param>
        /// <returns><c>true</c> if the attribute is found; otherwise, <c>false</c>.</returns>
        public static bool TryGetAttribute<T>(this ParameterInfo element, out T attribute, bool inherit) where T : Attribute
        {
            attribute = element.GetCustomAttributes(inherit)?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        /// <summary>Determines whether the enumeration value is decorated with an attribute of type <typeparamref name="T"/>.</summary>
        /// <remarks>Reads the attribute from the field representing the enumeration value.</remarks>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <returns><c>true</c> if the attribute is present; otherwise, <c>false</c>.</returns>
        public static bool HasAttribute<T>(this Enum element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

        /// <summary>Gets the first attribute of type <typeparamref name="T"/> applied to the enumeration value.</summary>
        /// <remarks>Reads the attribute from the field representing the enumeration value.</remarks>
        /// <typeparam name="T">The attribute type to look for.</typeparam>
        /// <param name="element">The element to inspect.</param>
        /// <param name="attribute">When this method returns, the first matching attribute, or <c>null</c>.</param>
        /// <returns><c>true</c> if the attribute is found; otherwise, <c>false</c>.</returns>
        public static bool TryGetAttribute<T>(this Enum element, out T attribute) where T : Attribute
        {
            attribute = element
                .GetType()
                .GetField(element.ToString())
                .GetCustomAttributes(false)?
                .OfType<T>()
                .FirstOrDefault();

            return attribute != null;
        }
    }
}