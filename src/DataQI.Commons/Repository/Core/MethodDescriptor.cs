using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using DataQI.Commons.Extensions.Reflection;

namespace DataQI.Commons.Repository.Core
{
    public sealed class MethodDescriptor
    {
        public MethodInfo Method { get; }
        public string UniqueName { get; }
        public DispatchKind Kind { get; }
        public MethodInfo ResolvedMethod { get; }
        public Type ResultType { get; }
        public bool HasCancellationToken { get; }

        private MethodDescriptor(MethodInfo method, string uniqueName, DispatchKind kind,
            MethodInfo resolvedMethod, Type resultType, bool hasCancellationToken)
        {
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

            if (exactMatchMethods.TryGetValue(uniqueName, out var exactMatch))
                return new MethodDescriptor(method, uniqueName, DispatchKind.ExactMatch,
                    exactMatch, method.ReturnType, hasCancellationToken);

            if (method.ReturnType.TryGetAsyncResultType(out var asyncResultType))
            {
                var isSingle = asyncResultType == entityType;
                var resolvedMethod = isSingle ? findOneAsyncMethod : findAsyncMethod;
                var kind = resolvedMethod == null ? DispatchKind.Unresolvable
                    : isSingle ? DispatchKind.AsyncSingle : DispatchKind.AsyncCollection;

                return new MethodDescriptor(method, uniqueName, kind, resolvedMethod, asyncResultType, hasCancellationToken);
            }

            var isSyncSingle = method.ReturnType == entityType;
            var syncResolvedMethod = isSyncSingle ? findOneMethod : findMethod;
            var syncKind = syncResolvedMethod == null ? DispatchKind.Unresolvable
                : isSyncSingle ? DispatchKind.SyncSingle : DispatchKind.SyncCollection;

            return new MethodDescriptor(method, uniqueName, syncKind, syncResolvedMethod, method.ReturnType, hasCancellationToken);
        }
    }
}
