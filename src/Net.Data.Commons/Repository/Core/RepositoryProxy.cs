using System;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Data.Commons.Repository.Core
{
    public class RepositoryProxy : DispatchProxy
    {
        protected static Func<Object> staticDefaultRepositoryFactory;

        protected readonly IDictionary<string, MethodInfo> defaultMethods;

        protected readonly Object defaultRepository;

        public RepositoryProxy()
        {
            defaultMethods = new Dictionary<string, MethodInfo>();
            defaultRepository = staticDefaultRepositoryFactory();

            if (defaultRepository == null)
                throw new ArgumentException("Repository must not be null");

            RegisterDefaultMethods();
        }

        public static TRepository Create<TRepository>(Func<Object> defaultRepositoryFactory)
        {
            staticDefaultRepositoryFactory = defaultRepositoryFactory;
            return Create<TRepository, RepositoryProxy>();
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (defaultMethods.TryGetValue(targetMethod.Name, out var method))
                return method.Invoke(defaultRepository, args);

            return null;
        }

        protected virtual void RegisterDefaultMethods()
        {
            RegisterDefaultMethod("Insert");
        }

        protected virtual void RegisterDefaultMethod(string methodName)
        {
            var method = defaultRepository.GetType().GetMethod(methodName);
            if (method != null)
                defaultMethods.Add(methodName, method);
        }
    }
}