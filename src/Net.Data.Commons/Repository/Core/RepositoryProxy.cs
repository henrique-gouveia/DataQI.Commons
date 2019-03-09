using System;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Data.Commons.Repository.Core
{
    public class RepositoryProxy : DispatchProxy
    {
        protected static Func<Object> DefaultRepositoryFactory;

        protected readonly IDictionary<string, MethodInfo> defaultMethods;

        protected readonly Object defaultRepository;

        public RepositoryProxy()
        {
            defaultMethods = new Dictionary<string, MethodInfo>();
            defaultRepository = DefaultRepositoryFactory();

            if (defaultRepository == null)
                throw new ArgumentException("Repository must not be null");

            RegisterDefaultMethods();
        }

        public static TRepository Create<TRepository>(Func<Object> defaultRepositoryFactory)
        {
            DefaultRepositoryFactory = defaultRepositoryFactory;
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
            RegisterDefaultMethod("Delete");
            RegisterDefaultMethod("DeleteAsync");
            RegisterDefaultMethod("Exists");
            RegisterDefaultMethod("ExistsAsync");
            RegisterDefaultMethod("FindAll");
            RegisterDefaultMethod("FindAllAsync");
            RegisterDefaultMethod("FindOne");
            RegisterDefaultMethod("FindOneAsync");
            RegisterDefaultMethod("Insert");
            RegisterDefaultMethod("InsertAsync");
            RegisterDefaultMethod("Save");
            RegisterDefaultMethod("SaveAsync");
        }

        protected virtual void RegisterDefaultMethod(string methodName)
        {
            var method = defaultRepository.GetType().GetMethod(methodName);
            if (method != null)
                defaultMethods.Add(methodName, method);
        }
    }
}