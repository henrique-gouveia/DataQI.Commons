using System;
using System.Linq;
using System.Reflection;

namespace DataQI.Commons.Test.Repository.Core
{
    internal static class FindImplementations
    {
        public static Func<string, Type[], MethodInfo> None { get; } = (name, parameterTypes) => null;

        public static Func<string, Type[], MethodInfo> Of(params (string Name, MethodInfo Method)[] methods)
            => (name, parameterTypes) => methods.FirstOrDefault(m => m.Name == name).Method;
    }
}
