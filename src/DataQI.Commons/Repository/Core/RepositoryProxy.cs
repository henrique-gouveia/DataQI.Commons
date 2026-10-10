using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<MethodInfo, MethodDescriptor> methodDescriptors =
            new ConcurrentDictionary<MethodInfo, MethodDescriptor>();
        private Func<MethodInfo, MethodDescriptor> methodDescriptorFactory;

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

            RegisterMethodDescriptors();
        }

        private void RegisterMethodDescriptors()
        {
            var exactMatchMethods = ResolveExactMatchMethods();
            var findMethod = ResolveCriteriaMethod("Find", 1);
            var findAsyncMethod = ResolveCriteriaMethod("FindAsync", 2);
            var findOneMethod = ResolveCriteriaMethod("FindOne", 1);
            var findOneAsyncMethod = ResolveCriteriaMethod("FindOneAsync", 2);

            methodDescriptorFactory = method => MethodDescriptor.Create(
                method,
                entityType,
                exactMatchMethods,
                findMethod,
                findAsyncMethod,
                findOneMethod,
                findOneAsyncMethod);

            foreach (var method in typeof(TRepository).GetAllInterfaceMethods())
                methodDescriptors[method] = methodDescriptorFactory(method);
        }

        private MethodInfo ResolveCriteriaMethod(string name, int parameterCount)
        {
            return defaultRepositoryType
                .GetMethods()
                .FirstOrDefault(m =>
                    m.Name == name &&
                    m.GetParameters().Length == parameterCount &&
                    m.GetParameters()[0].ParameterType.IsGenericType &&
                    m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(ICriteria) &&
                    m.GetParameters()[0].ParameterType.GetGenericArguments()[1] == typeof(ICriteria) &&
                    (parameterCount == 1 || m.GetParameters()[1].ParameterType == typeof(CancellationToken))
                );
        }

        private IDictionary<string, MethodInfo> ResolveExactMatchMethods()
        {
            var methods = new Dictionary<string, MethodInfo>();
            foreach (var method in defaultRepositoryType.GetInstancePublicMethods())
                if (method != null && !methods.ContainsKey(method.UniqueName()))
                    methods.Add(method.UniqueName(), method);
            return methods;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var descriptor = methodDescriptors.GetOrAdd(targetMethod, methodDescriptorFactory);

            switch (descriptor.Kind)
            {
                case DispatchKind.ExactMatch:
                    return descriptor.ResolvedMethod.Invoke(defaultRepository, args);

                case DispatchKind.AsyncCollection:
                case DispatchKind.AsyncSingle:
                {
                    var (criteriaArgs, cancellationToken) = descriptor.HasCancellationToken
                        ? SplitCancellationToken(args)
                        : (args, default);
                    var criteriaBuilder = CreateCriteriaBuilder(targetMethod, criteriaArgs);
                    return descriptor.ResolvedMethod.Invoke(defaultRepository, new object[] { criteriaBuilder, cancellationToken });
                }

                case DispatchKind.SyncCollection:
                case DispatchKind.SyncSingle:
                {
                    var criteriaBuilder = CreateCriteriaBuilder(targetMethod, args);
                    return descriptor.ResolvedMethod.Invoke(
                        defaultRepository, new object[] { criteriaBuilder });
                }

                default:
                    throw new TargetInvocationException(
                        $"Unknown method {targetMethod.Name} returning type {targetMethod.ReturnType}", null);
            }
        }

        protected virtual Func<ICriteria, ICriteria> CreateCriteriaBuilder(MethodInfo targetMethod, object[] args)
        {
            var plan = methodDescriptors.GetOrAdd(targetMethod, methodDescriptorFactory).Plan;

            ICriteria CriteriaBuilder(ICriteria criteria)
            {
                plan.ApplyTo(criteria, args);
                return criteria;
            }

            return CriteriaBuilder;
        }

        private static (object[] CriteriaArgs, CancellationToken CancellationToken) SplitCancellationToken(object[] args)
        {
            var cancellationToken = (CancellationToken)args[args.Length - 1];
            var criteriaArgs = args.Take(args.Length - 1).ToArray();
            return (criteriaArgs, cancellationToken);
        }
    }
}
