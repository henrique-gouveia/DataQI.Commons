using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Repository.Query;

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
        /// <summary>Gets the result type of the call: the return type, or the type wrapped by <c>Task</c> for parsed asynchronous query methods.</summary>
        public Type ResultType { get; }
        /// <summary>Gets a value indicating whether the last parameter of the method is a <see cref="System.Threading.CancellationToken"/>.</summary>
        public bool HasCancellationToken { get; }

        private MethodDescriptor(
            MethodInfo method,
            string uniqueName,
            DispatchKind kind,
            MethodInfo resolvedMethod,
            Type resultType,
            bool hasCancellationToken,
            Lazy<QueryPlan> plan)
        {
            this.plan = plan;
            Method = method;
            UniqueName = uniqueName;
            Kind = kind;
            ResolvedMethod = resolvedMethod;
            ResultType = resultType;
            HasCancellationToken = hasCancellationToken;
        }

        /// <summary>Creates the descriptor of an interface method.</summary>
        /// <param name="method">The interface method to describe.</param>
        /// <param name="entityType">The entity type of the repository.</param>
        /// <param name="exactMatchMethods">The public methods of the repository implementation, keyed by <see cref="UniqueName"/>.</param>
        /// <param name="findMethod">The implementation's <c>Find(Func&lt;ICriteria, ICriteria&gt;)</c>, or <c>null</c>.</param>
        /// <param name="findAsyncMethod">The implementation's <c>FindAsync</c> taking criteria and a token, or <c>null</c>.</param>
        /// <param name="findOneMethod">The implementation's <c>FindOne(Func&lt;ICriteria, ICriteria&gt;)</c>, or <c>null</c>.</param>
        /// <param name="findOneAsyncMethod">The implementation's <c>FindOneAsync</c> taking criteria and a token, or <c>null</c>.</param>
        /// <returns>The descriptor; an exact signature match wins over query method parsing.</returns>
        public static MethodDescriptor Create(
            MethodInfo method,
            Type entityType,
            IDictionary<string, MethodInfo> exactMatchMethods,
            MethodInfo findMethod,
            MethodInfo findAsyncMethod,
            MethodInfo findOneMethod,
            MethodInfo findOneAsyncMethod)
        {
            var uniqueName = method.UniqueName();
            var parameters = method.GetParameters();
            var hasCancellationToken = parameters.Length > 0 &&
                parameters[parameters.Length - 1].ParameterType == typeof(CancellationToken);

            if (TryResolveForExactMethod(
                method,
                uniqueName,
                hasCancellationToken,
                exactMatchMethods,
                out var exactDescriptor))
                return exactDescriptor;

            if (TryResolveForAsyncMethod(
                method,
                entityType,
                uniqueName,
                hasCancellationToken,
                findAsyncMethod,
                findOneAsyncMethod,
                out var asyncDescriptor))
                return asyncDescriptor;

            return ResolveForSyncMethod(
                method,
                entityType,
                uniqueName,
                hasCancellationToken,
                findMethod,
                findOneMethod);
        }

        private static bool TryResolveForExactMethod(
            MethodInfo method,
            string uniqueName,
            bool hasCancellationToken,
            IDictionary<string, MethodInfo> exactMatchMethods,
            out MethodDescriptor descriptor)
        {
            if (!exactMatchMethods.TryGetValue(uniqueName, out var exactMatch))
            {
                descriptor = null;
                return false;
            }

            descriptor = new MethodDescriptor(
                method,
                uniqueName,
                DispatchKind.ExactMatch,
                exactMatch,
                method.ReturnType,
                hasCancellationToken,
                null);
            return true;
        }

        private static bool TryResolveForAsyncMethod(
            MethodInfo method,
            Type entityType,
            string uniqueName,
            bool hasCancellationToken,
            MethodInfo findAsyncMethod,
            MethodInfo findOneAsyncMethod,
            out MethodDescriptor descriptor)
        {
            if (!method.ReturnType.TryGetAsyncResultType(out var asyncResultType))
            {
                descriptor = null;
                return false;
            }

            var isSingle = asyncResultType == entityType;
            var resolvedMethod = isSingle ? findOneAsyncMethod : findAsyncMethod;
            var kind = resolvedMethod == null
                ? DispatchKind.Unresolvable
                : isSingle
                    ? DispatchKind.AsyncSingle
                    : DispatchKind.AsyncCollection;

            descriptor = new MethodDescriptor(
                method,
                uniqueName,
                kind,
                resolvedMethod,
                asyncResultType,
                hasCancellationToken,
                kind == DispatchKind.Unresolvable ? null : new Lazy<QueryPlan>(() => QueryMethodParser.Parse(method)));
            return true;
        }

        private static MethodDescriptor ResolveForSyncMethod(
            MethodInfo method,
            Type entityType,
            string uniqueName,
            bool hasCancellationToken,
            MethodInfo findMethod,
            MethodInfo findOneMethod)
        {
            var isSyncSingle = method.ReturnType == entityType;
            var syncResolvedMethod = isSyncSingle
                ? findOneMethod
                : findMethod;
            var syncKind = syncResolvedMethod == null
                ? DispatchKind.Unresolvable
                : isSyncSingle
                    ? DispatchKind.SyncSingle
                    : DispatchKind.SyncCollection;

            return new MethodDescriptor(
                method,
                uniqueName,
                syncKind,
                syncResolvedMethod,
                method.ReturnType,
                hasCancellationToken,
                syncKind == DispatchKind.Unresolvable ? null : new Lazy<QueryPlan>(() => QueryMethodParser.Parse(method)));
        }
    }
}
