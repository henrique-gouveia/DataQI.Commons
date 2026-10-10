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
    /// <summary>Implements a repository interface by forwarding calls to a provider implementation.</summary>
    /// <typeparam name="TRepository">The repository interface.</typeparam>
    /// <remarks>
    /// Calls that match a public method of the implementation are forwarded unchanged. Other calls are treated as
    /// query methods: the method name is parsed by <see cref="DataQI.Commons.Repository.Query.QueryMethodParser"/> and the resulting criteria
    /// are passed to the implementation's <c>Find</c>, <c>FindAsync</c>, <c>FindOne</c> or <c>FindOneAsync</c>.
    /// A call that cannot be routed throws <see cref="System.Reflection.TargetInvocationException"/>.
    /// </remarks>
    public class RepositoryProxy<TRepository> : DispatchProxy where TRepository : class
    {
        private static readonly object createLock = new object();

        /// <summary>Stores the factory for the proxy being constructed, set by <see cref="Create(Func{object})"/>.</summary>
        protected static Func<object> DefaultRepositoryFactory;

        /// <summary>Stores the implementation the proxy forwards to.</summary>
        protected readonly object defaultRepository;
        /// <summary>Stores the runtime type of <see cref="defaultRepository"/>.</summary>
        protected readonly Type defaultRepositoryType;
        /// <summary>Stores the entity type of <typeparamref name="TRepository"/>.</summary>
        protected readonly Type entityType;

        private readonly ConcurrentDictionary<MethodInfo, MethodDescriptor> methodDescriptors =
            new ConcurrentDictionary<MethodInfo, MethodDescriptor>();
        private Func<MethodInfo, MethodDescriptor> methodDescriptorFactory;

        /// <summary>Creates a proxy that implements <typeparamref name="TRepository"/>.</summary>
        /// <param name="defaultRepositoryFactory">A function that returns the implementation the proxy forwards to.</param>
        /// <returns>The proxy instance.</returns>
        public static TRepository Create(Func<object> defaultRepositoryFactory)
        {
            lock (createLock)
            {
                DefaultRepositoryFactory = defaultRepositoryFactory;
                return Create<TRepository, RepositoryProxy<TRepository>>();
            }
        }

        /// <summary>Initializes the proxy when called by <see cref="DispatchProxy"/>, with construction initiated through <see cref="Create(Func{object})"/>.</summary>
        /// <exception cref="System.ArgumentException">The factory returned <c>null</c>.</exception>
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

        /// <summary>Routes a call made on the repository interface.</summary>
        /// <param name="targetMethod">The interface method that was called.</param>
        /// <param name="args">The call arguments.</param>
        /// <returns>The value returned by the implementation.</returns>
        /// <exception cref="System.Reflection.TargetInvocationException">The method cannot be routed (<see cref="DispatchKind.Unresolvable"/>).</exception>
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

        /// <summary>Creates the criteria builder that applies a query method's plan to its arguments.</summary>
        /// <param name="targetMethod">The query method that was called.</param>
        /// <param name="args">The call arguments, without the cancellation token.</param>
        /// <returns>A function that fills the given <see cref="DataQI.Commons.Query.ICriteria"/>.</returns>
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
