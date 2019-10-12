using System;
using System.Linq;
using System.Reflection;

namespace DataQI.Commons.Extensions.Reflection
{
    public static class CustomAttributeExtensions
    {
        public static bool HasAttribute<T>(this Assembly element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

        public static bool TryGetAttribute<T>(this Assembly element, out T attribute) where T : Attribute
        {
            attribute = element.GetCustomAttributes()?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        public static bool HasAttribute<T>(this Module element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

        public static bool TryGetAttribute<T>(this Module element, out T attribute) where T : Attribute
        {
            attribute = element.GetCustomAttributes()?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        public static bool HasAttribute<T>(this MemberInfo element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

        public static bool TryGetAttribute<T>(this MemberInfo element, out T attribute) where T : Attribute
        {
            attribute = element.GetCustomAttributes()?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        public static bool HasAttribute<T>(this ParameterInfo element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

        public static bool TryGetAttribute<T>(this ParameterInfo element, out T attribute) where T : Attribute
        {
            attribute = element.GetCustomAttributes()?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        public static bool HasAttribute<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute, inherit);
        }

        public static bool TryGetAttribute<T>(this MemberInfo element, out T attribute, bool inherit) where T : Attribute
        {
            attribute = element.GetCustomAttributes(inherit)?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        public static bool HasAttribute<T>(this ParameterInfo element, bool inherit) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute, inherit);
        }

        public static bool TryGetAttribute<T>(this ParameterInfo element, out T attribute, bool inherit) where T : Attribute
        {
            attribute = element.GetCustomAttributes(inherit)?.OfType<T>().FirstOrDefault();
            return attribute != null;
        }

        public static bool HasAttribute<T>(this Enum element) where T : Attribute
        {
            T attribute;
            return TryGetAttribute(element, out attribute);
        }

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