using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Query;
using DataQI.Commons.Repository.Query;
using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Core
{
    public class RepositoryProxy<TRepository> : DispatchProxy where TRepository : class
    {
        private static readonly object createLock = new object();

        protected static Func<object> DefaultRepositoryFactory;

        protected readonly object defaultRepository;
        protected readonly Type defaultRepositoryType;

        protected readonly IDictionary<string, MethodInfo> defaultRepositoryMethods = new Dictionary<string, MethodInfo>();
        protected MethodInfo defaultFindByCriteriaMethod;

        public static TRepository Create(Func<object> defaultRepositoryFactory)
        {
            // DefaultRepositoryFactory is static (shared by every RepositoryProxy<TRepository>
            // instance for this closed generic type), because DispatchProxy.Create<T, TProxy>()
            // offers no way to pass constructor arguments. The lock keeps the write and the
            // constructor's read of it atomic with respect to concurrent Create calls for the
            // same TRepository, so one caller's factory can never leak into another caller's proxy.
            lock (createLock)
            {
                DefaultRepositoryFactory = defaultRepositoryFactory;
                return Create<TRepository, RepositoryProxy<TRepository>>();
            }
        }

        public RepositoryProxy()
        {
            defaultRepository = DefaultRepositoryFactory();
            defaultRepositoryType = defaultRepository?.GetType();

            Assert.NotNull(defaultRepository, "Repository must not be null");

            RegisterDefaultRepositoryMethods();
            RegisterDefaultFindByCriteriaMethod();
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (TryGetDefaultMethod(targetMethod.UniqueName(), out var method))
                return method.Invoke(defaultRepository, args);

            if (defaultFindByCriteriaMethod != null)
            {
                var criteriaBuilder = CreateCriteriaBuilder(targetMethod, args);
                return defaultFindByCriteriaMethod.Invoke(defaultRepository, new object[] { criteriaBuilder });
            }

            throw new TargetInvocationException(
                $"Unknown method {targetMethod.Name} returning type {targetMethod.ReturnType}", null);
        }

        protected virtual Func<ICriteria, ICriteria> CreateCriteriaBuilder(MethodInfo targetMethod, object[] args)
        {
            ICriteria CriteriaBuilder(ICriteria criteria)
            {
                var factory = new QueryFactory(targetMethod, args);
                factory.BuildCriteria(criteria);

                return criteria;
            }

            return CriteriaBuilder;
        }
        
        private void RegisterDefaultFindByCriteriaMethod()
        {
            defaultFindByCriteriaMethod = defaultRepositoryType
                .GetMethods()
                .FirstOrDefault(m => 
                    m.Name == "Find" &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType.IsGenericType &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(ICriteria) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[1] == typeof(ICriteria)
                );          
        }

        private void RegisterDefaultRepositoryMethods()
        {
            MethodInfo[] methods = defaultRepositoryType.GetInstancePublicMethods();
            foreach (var method in methods)
                RegisterMethod(method);
        }

        protected virtual void RegisterMethod(MethodInfo method)
        {
            if (method != null && !defaultRepositoryMethods.ContainsKey(method.Name))
                defaultRepositoryMethods.Add(method.UniqueName(), method);
        }

        protected virtual bool TryGetDefaultMethod(string name, out MethodInfo method)
            => defaultRepositoryMethods.TryGetValue(name, out method);
    }
}