using System;
using System.Collections.Generic;
using System.Reflection;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Query;
using DataQI.Commons.Repository.Query;
using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Core
{
    public class RepositoryProxy<TRepository> : DispatchProxy where TRepository : class
    {
        protected static Func<object> DefaultRepositoryFactory;

        protected readonly object defaultRepository;
        protected readonly Type defaultRepositoryType;
        protected readonly IDictionary<string, MethodInfo> defaultRepositoryMethods;

        public static TRepository Create(Func<object> defaultRepositoryFactory)
        {
            DefaultRepositoryFactory = defaultRepositoryFactory;
            return Create<TRepository, RepositoryProxy<TRepository>>();
        }

        public RepositoryProxy()
        {
            defaultRepository = DefaultRepositoryFactory();
            defaultRepositoryType = defaultRepository?.GetType();

            Assert.NotNull(defaultRepository, "Repository must not be null");

            defaultRepositoryMethods = new Dictionary<string, MethodInfo>();
            RegisterDefaultRepositoryMethods();
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (TryGetDefaultMethod(targetMethod.Name, out var method))
                return method.Invoke(defaultRepository, args);

            if (TryGetDefaultMethod("Find", out method))
            {
                var criteriaBuilder = CreateCriteriaBuilder(targetMethod, args);
                return method.Invoke(defaultRepository, new object[] { criteriaBuilder });
            }

            throw new TargetInvocationException(
                $"Unknown method {targetMethod.Name} return type {targetMethod.ReturnType}", null);
        }

        protected virtual Func<ICriteria, ICriteria> CreateCriteriaBuilder(MethodInfo targetMethod, object[] args)
        {
            ICriteria criteriaBuilder(ICriteria criteria)
            {
                var factory = new QueryFactory(targetMethod, args);
                factory.BuildCriteria(criteria);

                return criteria;
            }

            return criteriaBuilder;
        }
        
        private void RegisterDefaultRepositoryMethods()
        {
            MethodInfo[] methods = defaultRepositoryType.GetInstancePublicMethods();
            foreach (var method in methods)
                RegisterMethod( method);
        }

        protected virtual void RegisterMethod(MethodInfo method)
        {
            if (defaultRepositoryMethods.ContainsKey(method.Name)) return;
            
            if (method != null)
                defaultRepositoryMethods.Add(method.Name, method);
        }

        protected bool TryGetDefaultMethod(string name, out MethodInfo method)
            => defaultRepositoryMethods.TryGetValue(name, out method);
    }
}