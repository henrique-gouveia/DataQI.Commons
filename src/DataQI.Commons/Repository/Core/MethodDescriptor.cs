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
        /// <summary>Gets the result type of the call: the return type, or the type wrapped by <c>Task</c> for parsed asynchronous query methods.</summary>
        public Type ResultType { get; }
        /// <summary>Gets a value indicating whether the last parameter of the method is a <see cref="System.Threading.CancellationToken"/>.</summary>
        public bool HasCancellationToken { get; }

        /// <summary>Gets the page request parameter index, or -1 when absent.</summary>
        public int PageRequestIndex { get; }

        private MethodDescriptor(
            MethodInfo method,
            string uniqueName,
            DispatchKind kind,
            MethodInfo resolvedMethod,
            Type resultType,
            bool hasCancellationToken,
            Lazy<QueryPlan> plan,
            int pageRequestIndex = -1)
        {
            this.plan = plan;
            PageRequestIndex = pageRequestIndex;
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
            => Create(method, entityType, exactMatchMethods, findMethod, findAsyncMethod, findOneMethod, findOneAsyncMethod, PagedMethods.None);

        /// <summary>Creates a descriptor with optional paging implementation methods.</summary>
        public static MethodDescriptor Create(
            MethodInfo method,
            Type entityType,
            IDictionary<string, MethodInfo> exactMatchMethods,
            MethodInfo findMethod,
            MethodInfo findAsyncMethod,
            MethodInfo findOneMethod,
            MethodInfo findOneAsyncMethod,
            PagedMethods pagedMethods)
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

            var isPaged = TryGetPagedShape(method.ReturnType, entityType, out var isAsync, out var isPage);
            var resolved = isPaged ? (isPage
                ? (isAsync ? pagedMethods.FindPageAsync : pagedMethods.FindPage)
                : (isAsync ? pagedMethods.FindSliceAsync : pagedMethods.FindSlice)) : null;
            ValidatePagingSignature(method, isPaged, hasCancellationToken, resolved);
            if (isPaged)
            {
                var kind = resolved == null ? DispatchKind.Unresolvable
                    : isPage ? (isAsync ? DispatchKind.AsyncPage : DispatchKind.SyncPage)
                    : (isAsync ? DispatchKind.AsyncSlice : DispatchKind.SyncSlice);
                var resultType = isAsync ? method.ReturnType.GetGenericArguments()[0] : method.ReturnType;
                var pageIndex = Array.FindIndex(parameters, p => p.ParameterType == typeof(IPageRequest));
                return new MethodDescriptor(method, uniqueName, kind, resolved, resultType, hasCancellationToken,
                    resolved == null ? null : new Lazy<QueryPlan>(() => QueryMethodParser.Parse(method)), pageIndex);
            }

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

        private static void ValidatePagingSignature(MethodInfo method, bool isPaged, bool hasCancellationToken, MethodInfo resolved)
        {
            var parameters = method.GetParameters();
            var requestPositions = parameters
                .Select((p, i) => new { p.ParameterType, Index = i })
                .Where(x => x.ParameterType == typeof(IPageRequest))
                .Select(x => x.Index)
                .ToArray();

            string Fail(string reason)
                => $"{method.DeclaringType?.Name}.{method.Name}: {reason}";

            if (requestPositions.Length > 1)
                throw new InvalidOperationException(Fail("a query method accepts at most one IPageRequest parameter"));
            if (isPaged && requestPositions.Length == 0)
                throw new InvalidOperationException(Fail("a method returning ISlice<T> or IPage<T> needs an IPageRequest parameter"));
            if (!isPaged && requestPositions.Length == 1)
                throw new InvalidOperationException(Fail("an IPageRequest parameter requires an ISlice<T> or IPage<T> return type"));
            if (requestPositions.Length == 1)
            {
                var lastAllowed = parameters.Length - (hasCancellationToken ? 2 : 1);
                if (requestPositions[0] != lastAllowed)
                    throw new InvalidOperationException(Fail("the IPageRequest parameter must be the last one, before an optional CancellationToken"));
            }
            if (isPaged)
            {
                var tokenPositions = parameters.Select((p, i) => new { p.ParameterType, Index = i })
                    .Where(x => x.ParameterType == typeof(CancellationToken)).Select(x => x.Index).ToArray();
                if (tokenPositions.Length > 1 || (tokenPositions.Length == 1 && !hasCancellationToken))
                    throw new InvalidOperationException(Fail("an optional CancellationToken must occur once at the end"));
            }
            if (isPaged && resolved == null)
                throw new InvalidOperationException(Fail("the repository does not implement IPagedRepository"));
        }

        private static bool TryGetPagedShape(Type returnType, Type entityType, out bool isAsync, out bool isPage)
        {
            var type = returnType;
            isAsync = type.TryGetAsyncResultType(out var inner);
            if (isAsync)
                type = inner;
            isPage = false;
            if (!type.IsGenericType || type.GetGenericArguments()[0] != entityType)
                return false;
            var definition = type.GetGenericTypeDefinition();
            if (definition == typeof(ISlice<>))
                return true;
            if (definition != typeof(IPage<>))
                return false;
            isPage = true;
            return true;
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
