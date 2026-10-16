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
        /// <summary>Gets a value indicating whether the method has an <see cref="IPageRequest"/> parameter, always followed at most by a <see cref="CancellationToken"/>.</summary>
        public bool HasPageRequest { get; }
        /// <summary>Gets a value indicating whether the last parameter of the method is a <see cref="System.Threading.CancellationToken"/>.</summary>
        public bool HasCancellationToken { get; }

        private MethodDescriptor(
            MethodInfo method,
            string uniqueName,
            DispatchKind kind,
            MethodInfo resolvedMethod,
            Type resultType,
            bool hasPageRequest,
            bool hasCancellationToken,
            Lazy<QueryPlan> plan)
        {
            this.plan = plan;
            Method = method;
            UniqueName = uniqueName;
            Kind = kind;
            ResolvedMethod = resolvedMethod;
            ResultType = resultType;
            HasPageRequest = hasPageRequest;
            HasCancellationToken = hasCancellationToken;
        }

        /// <summary>Creates the descriptor of an interface method.</summary>
        /// <param name="method">The interface method to describe.</param>
        /// <param name="entityType">The entity type of the repository.</param>
        /// <param name="exactMatchMethods">The public methods of the repository implementation, keyed by <see cref="UniqueName"/>.</param>
        /// <param name="findMethods">The <c>Find*</c> methods of the repository implementation, any of which may be <c>null</c>.</param>
        /// <returns>The descriptor; an exact signature match wins over query method parsing.</returns>
        public static MethodDescriptor Create(
            MethodInfo method,
            Type entityType,
            IDictionary<string, MethodInfo> exactMatchMethods,
            FindMethods findMethods)
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

            var isPaged = TryGetPagedShape(method, entityType, out var isAsync, out var isPage);
            var resolved = isPaged ? (isPage
                ? (isAsync ? findMethods.FindPageAsync : findMethods.FindPage)
                : (isAsync ? findMethods.FindSliceAsync : findMethods.FindSlice)) : null;
            ValidatePagingSignature(method, isPaged, hasCancellationToken, resolved);
            if (isPaged)
            {
                var kind = resolved == null ? DispatchKind.Unresolvable
                    : isPage ? (isAsync ? DispatchKind.AsyncPage : DispatchKind.SyncPage)
                    : (isAsync ? DispatchKind.AsyncSlice : DispatchKind.SyncSlice);
                var resultType = isAsync ? method.ReturnType.GetGenericArguments()[0] : method.ReturnType;
                return new MethodDescriptor(method, uniqueName, kind, resolved, resultType, true, hasCancellationToken,
                    resolved == null ? null : new Lazy<QueryPlan>(() => QueryMethodParser.Parse(method)));
            }

            if (TryResolveForAsyncMethod(
                method,
                entityType,
                uniqueName,
                hasCancellationToken,
                findMethods.FindAsync,
                findMethods.FindOneAsync,
                out var asyncDescriptor))
                return asyncDescriptor;

            return ResolveForSyncMethod(
                method,
                entityType,
                uniqueName,
                hasCancellationToken,
                findMethods.Find,
                findMethods.FindOne);
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

        private static bool TryGetPagedShape(MethodInfo method, Type entityType, out bool isAsync, out bool isPage)
        {
            var type = method.ReturnType;
            isAsync = type.TryGetAsyncResultType(out var inner);
            if (isAsync)
                type = inner;
            isPage = false;
            if (!type.IsGenericType)
                return false;
            var definition = type.GetGenericTypeDefinition();
            if (definition != typeof(ISlice<>) && definition != typeof(IPage<>))
                return false;
            var itemType = type.GetGenericArguments()[0];
            if (itemType != entityType)
                throw new InvalidOperationException(
                    $"{method.DeclaringType?.Name}.{method.Name}: paging result T must be the repository entity {entityType.FullName}, but was {itemType.FullName}");
            isPage = definition == typeof(IPage<>);
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
                false,
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
                false,
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
                false,
                hasCancellationToken,
                syncKind == DispatchKind.Unresolvable ? null : new Lazy<QueryPlan>(() => QueryMethodParser.Parse(method)));
        }
    }
}
