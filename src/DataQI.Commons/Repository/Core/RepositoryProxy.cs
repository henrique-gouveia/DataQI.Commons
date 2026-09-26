using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

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
        protected readonly Type entityType;

        protected readonly IDictionary<string, MethodInfo> defaultRepositoryMethods = new Dictionary<string, MethodInfo>();
        protected MethodInfo defaultFindByCriteriaMethod;
        protected MethodInfo defaultFindByCriteriaAsyncMethod;
        protected MethodInfo defaultFindOneByCriteriaMethod;
        protected MethodInfo defaultFindOneByCriteriaAsyncMethod;

        public static TRepository Create(Func<object> defaultRepositoryFactory)
        {
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

            entityType = new RepositoryMetadata(typeof(TRepository)).EntityType;

            RegisterDefaultRepositoryMethods();
            RegisterDefaultFindByCriteriaMethod();
            RegisterDefaultFindByCriteriaAsyncMethod();
            RegisterDefaultFindOneByCriteriaMethod();
            RegisterDefaultFindOneByCriteriaAsyncMethod();
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (TryResolveInvocableMethod(targetMethod, args, out var method, out var invokeArgs))
                return method.Invoke(defaultRepository, invokeArgs);

            throw new TargetInvocationException(
                $"Unknown method {targetMethod.Name} returning type {targetMethod.ReturnType}", null);
        }

        protected virtual bool TryResolveInvocableMethod(
            MethodInfo targetMethod, object[] args, out MethodInfo method, out object[] invokeArgs)
        {
            if (TryGetDefaultMethod(targetMethod.UniqueName(), out method))
            {
                invokeArgs = args;
                return true;
            }

            if (targetMethod.ReturnType.TryGetAsyncResultType(out var asyncResultType))
            {
                method = asyncResultType == entityType ? defaultFindOneByCriteriaAsyncMethod : defaultFindByCriteriaAsyncMethod;
                if (method == null) { invokeArgs = null; return false; }

                var (criteriaArgs, cancellationToken) = SplitCancellationToken(targetMethod, args);
                invokeArgs = new object[] { CreateCriteriaBuilder(targetMethod, criteriaArgs), cancellationToken };
                return true;
            }

            method = targetMethod.ReturnType == entityType ? defaultFindOneByCriteriaMethod : defaultFindByCriteriaMethod;
            if (method == null) { invokeArgs = null; return false; }

            invokeArgs = new object[] { CreateCriteriaBuilder(targetMethod, args) };
            return true;
        }

        private static (object[] CriteriaArgs, CancellationToken CancellationToken) SplitCancellationToken(
            MethodInfo targetMethod, object[] args)
        {
            var parameters = targetMethod.GetParameters();
            var hasCancellationToken = parameters.Length > 0 &&
                parameters[parameters.Length - 1].ParameterType == typeof(CancellationToken);

            var cancellationToken = hasCancellationToken
                ? (CancellationToken)args[args.Length - 1]
                : default;
            var criteriaArgs = hasCancellationToken
                ? args.Take(args.Length - 1).ToArray()
                : args;

            return (criteriaArgs, cancellationToken);
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

        private void RegisterDefaultFindByCriteriaAsyncMethod()
        {
            defaultFindByCriteriaAsyncMethod = defaultRepositoryType
                .GetMethods()
                .FirstOrDefault(m =>
                    m.Name == "FindAsync" &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.IsGenericType &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(ICriteria) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[1] == typeof(ICriteria) &&
                    m.GetParameters()[1].ParameterType == typeof(CancellationToken)
                );
        }

        private void RegisterDefaultFindOneByCriteriaMethod()
        {
            defaultFindOneByCriteriaMethod = defaultRepositoryType
                .GetMethods()
                .FirstOrDefault(m =>
                    m.Name == "FindOne" &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType.IsGenericType &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(ICriteria) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[1] == typeof(ICriteria)
                );
        }

        private void RegisterDefaultFindOneByCriteriaAsyncMethod()
        {
            defaultFindOneByCriteriaAsyncMethod = defaultRepositoryType
                .GetMethods()
                .FirstOrDefault(m =>
                    m.Name == "FindOneAsync" &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.IsGenericType &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(ICriteria) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[1] == typeof(ICriteria) &&
                    m.GetParameters()[1].ParameterType == typeof(CancellationToken)
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
