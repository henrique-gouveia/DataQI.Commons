using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Repository.Query;

namespace DataQI.Commons.Repository.Core
{
    public sealed class MethodDescriptor
    {
        private readonly Lazy<QueryPlan> plan;

        public QueryPlan Plan => plan?.Value;

        public MethodInfo Method { get; }
        public string UniqueName { get; }
        public DispatchKind Kind { get; }
        public MethodInfo ResolvedMethod { get; }
        public Type ResultType { get; }
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
