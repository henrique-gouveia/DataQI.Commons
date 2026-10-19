using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Repository.Query;
using DataQI.Commons.Repository.Paging;

namespace DataQI.Commons.Repository.Core
{
    /// <summary>Describes how a method of a repository interface is dispatched.</summary>
    public sealed class MethodDescriptor
    {
        private readonly Lazy<QueryPlan> plan;

        /// <summary>Gets the query plan parsed on first access, or <c>null</c> when the method is forwarded unchanged or cannot be resolved.</summary>
        public QueryPlan Plan => plan?.Value;

        /// <summary>Gets the interface method being dispatched.</summary>
        public MethodInfo Method { get; }
        /// <summary>Gets a key made of the method name, return type and parameter types.</summary>
        public string UniqueName { get; }
        /// <summary>Gets how the call is routed.</summary>
        public DispatchKind Kind { get; }
        /// <summary>Gets the repository implementation method the call is forwarded to, or <c>null</c> when <see cref="Kind"/> is <see cref="DispatchKind.Unresolvable"/>.</summary>
        public MethodInfo ResolvedMethod { get; }
        /// <summary>Gets the result type of the call: the return type, or the type wrapped by <see cref="System.Threading.Tasks.Task{TResult}"/> for parsed asynchronous query methods.</summary>
        public Type ResultType { get; }
        /// <summary>Gets a value indicating whether the method has an <see cref="IPageRequest"/> parameter, always followed at most by a <see cref="CancellationToken"/>.</summary>
        public bool HasPageRequest => Shape?.HasPageRequest ?? false;
        /// <summary>Gets a value indicating whether the last parameter of the method is a <see cref="System.Threading.CancellationToken"/>.</summary>
        public bool HasCancellationToken { get; }

        /// <summary>Gets the shape of the query method, or <c>null</c> when the method is forwarded unchanged or cannot be resolved.</summary>
        public QueryShape Shape { get; }

        private MethodDescriptor(
            MethodInfo method,
            string uniqueName,
            DispatchKind kind,
            MethodInfo resolvedMethod,
            Type resultType,
            bool hasCancellationToken,
            QueryShape shape,
            Lazy<QueryPlan> plan)
        {
            Method = method;
            UniqueName = uniqueName;
            Kind = kind;
            ResolvedMethod = resolvedMethod;
            ResultType = resultType;
            HasCancellationToken = hasCancellationToken;
            Shape = shape;
            this.plan = plan;
        }

        /// <summary>Creates the descriptor of an interface method.</summary>
        /// <param name="method">The interface method to describe.</param>
        /// <param name="entityType">The entity type of the repository.</param>
        /// <param name="exactMatchMethods">The public methods of the repository implementation, keyed by <see cref="UniqueName"/>.</param>
        /// <param name="findImplementation">Finds the repository method that serves a query method, given its name and the parameter types that follow the criteria; returns <c>null</c> when the repository has none.</param>
        /// <returns>The descriptor; an exact signature match wins over query method parsing.</returns>
        public static MethodDescriptor Create(
            MethodInfo method,
            Type entityType,
            IDictionary<string, MethodInfo> exactMatchMethods,
            Func<string, Type[], MethodInfo> findImplementation)
        {
            var uniqueName = method.UniqueName();
            var parameters = method.GetParameters();
            var hasCancellationToken = parameters.Length > 0 &&
                parameters[parameters.Length - 1].ParameterType == typeof(CancellationToken);

            if (exactMatchMethods.TryGetValue(uniqueName, out var exactMatch))
                return new MethodDescriptor(
                    method, uniqueName, DispatchKind.ExactMatch, exactMatch, method.ReturnType,
                    hasCancellationToken, null, null);

            var shape = QueryShape.Classify(method, entityType);
            var resolved = findImplementation(shape.ImplementationName, shape.ImplementationParameterTypes);
            ValidatePagingSignature(method, shape.HasPageRequest, hasCancellationToken, resolved);

            if (resolved == null)
                return new MethodDescriptor(
                    method, uniqueName, DispatchKind.Unresolvable, null, shape.ResultType,
                    hasCancellationToken, null, null);

            return new MethodDescriptor(
                method, uniqueName, shape.Kind, resolved, shape.ResultType,
                hasCancellationToken, shape, new Lazy<QueryPlan>(() => QueryMethodParser.Parse(method)));
        }

        private static void ValidatePagingSignature(MethodInfo method, bool isPaged, bool hasCancellationToken, MethodInfo resolved)
        {
            var parameters = method.GetParameters();
            var requestPositions = parameters
                .Select((p, i) => new { p.ParameterType, Index = i })
                .Where(x => x.ParameterType == typeof(IPageRequest))
                .Select(x => x.Index)
                .ToArray();

            string Message(string reason) => $"{method.DeclaringType?.Name}.{method.Name}: {reason}";

            if (requestPositions.Length > 1)
                throw new InvalidOperationException(Message("a query method accepts at most one IPageRequest parameter"));
            if (isPaged && requestPositions.Length == 0)
                throw new InvalidOperationException(Message("a method returning ISlice<T> or IPage<T> needs an IPageRequest parameter"));
            if (!isPaged && requestPositions.Length == 1)
                throw new InvalidOperationException(Message("an IPageRequest parameter requires an ISlice<T> or IPage<T> return type"));
            if (requestPositions.Length == 1)
            {
                var lastAllowed = parameters.Length - (hasCancellationToken ? 2 : 1);
                if (requestPositions[0] != lastAllowed)
                    throw new InvalidOperationException(Message("the IPageRequest parameter must be the last one, before an optional CancellationToken"));
            }
            if (isPaged)
            {
                var tokenPositions = parameters.Select((p, i) => new { p.ParameterType, Index = i })
                    .Where(x => x.ParameterType == typeof(CancellationToken)).Select(x => x.Index).ToArray();
                if (tokenPositions.Length > 1 || (tokenPositions.Length == 1 && !hasCancellationToken))
                    throw new InvalidOperationException(Message("an optional CancellationToken must occur once at the end"));
            }
            if (isPaged && resolved == null)
                throw new InvalidOperationException(Message("the repository does not implement IPagedRepository"));
        }
    }
}
