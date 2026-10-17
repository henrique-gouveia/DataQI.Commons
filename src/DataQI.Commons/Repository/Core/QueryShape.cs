using System;
using System.Reflection;
using System.Threading;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Query;
using DataQI.Commons.Repository.Paging;

namespace DataQI.Commons.Repository.Core
{
    internal enum QueryResult
    {
        Collection,
        Single,
        Slice,
        Page
    }

    /// <summary>Describes the form of a query method: what it returns and whether it is asynchronous.</summary>
    internal sealed class QueryShape
    {
        private QueryShape(QueryResult result, bool isAsync, Type resultType)
        {
            Result = result;
            IsAsync = isAsync;
            ResultType = resultType;
        }

        /// <summary>Gets what the query method returns.</summary>
        public QueryResult Result { get; }
        /// <summary>Gets a value indicating whether the query method is asynchronous.</summary>
        public bool IsAsync { get; }
        /// <summary>Gets the result type of the call, without the asynchronous wrapper.</summary>
        public Type ResultType { get; }
        /// <summary>Gets a value indicating whether the query method takes an <see cref="IPageRequest"/>.</summary>
        public bool HasPageRequest => Result == QueryResult.Slice || Result == QueryResult.Page;

        /// <summary>Gets the name of the repository method the query method is forwarded to.</summary>
        public string ImplementationName => $"Find{ResultSuffix}{(IsAsync ? "Async" : string.Empty)}";

        /// <summary>Gets the parameter types that follow the criteria in the repository method.</summary>
        public Type[] ImplementationParameterTypes => HasPageRequest
            ? (IsAsync ? new[] { typeof(IPageRequest), typeof(CancellationToken) } : new[] { typeof(IPageRequest) })
            : (IsAsync ? new[] { typeof(CancellationToken) } : Array.Empty<Type>());

        /// <summary>Gets how the call is routed when the repository method exists.</summary>
        public DispatchKind Kind
        {
            get
            {
                switch (Result)
                {
                    case QueryResult.Single:
                        return IsAsync ? DispatchKind.AsyncSingle : DispatchKind.SyncSingle;
                    case QueryResult.Slice:
                        return IsAsync ? DispatchKind.AsyncSlice : DispatchKind.SyncSlice;
                    case QueryResult.Page:
                        return IsAsync ? DispatchKind.AsyncPage : DispatchKind.SyncPage;
                    default:
                        return IsAsync ? DispatchKind.AsyncCollection : DispatchKind.SyncCollection;
                }
            }
        }

        private string ResultSuffix
        {
            get
            {
                switch (Result)
                {
                    case QueryResult.Single:
                        return "One";
                    case QueryResult.Slice:
                        return "Slice";
                    case QueryResult.Page:
                        return "Page";
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>Builds the arguments of the repository method call.</summary>
        public object[] BuildArguments(
            Func<ICriteria, ICriteria> criteriaBuilder,
            IPageRequest page,
            CancellationToken cancellationToken)
        {
            var arguments = new object[1 + (HasPageRequest ? 1 : 0) + (IsAsync ? 1 : 0)];
            var index = 0;
            arguments[index++] = criteriaBuilder;
            if (HasPageRequest)
                arguments[index++] = page;
            if (IsAsync)
                arguments[index] = cancellationToken;
            return arguments;
        }

        /// <summary>Classifies a query method of a repository interface.</summary>
        /// <exception cref="InvalidOperationException">A paged result is not of the repository entity.</exception>
        public static QueryShape Classify(MethodInfo method, Type entityType)
        {
            var type = method.ReturnType;
            var isAsync = type.TryGetAsyncResultType(out var inner);
            if (isAsync)
                type = inner;

            if (type.IsGenericType)
            {
                var definition = type.GetGenericTypeDefinition();
                if (definition == typeof(ISlice<>) || definition == typeof(IPage<>))
                {
                    var itemType = type.GetGenericArguments()[0];
                    if (itemType != entityType)
                        throw new InvalidOperationException(
                            $"{method.DeclaringType?.Name}.{method.Name}: paging result T must be the repository entity {entityType.FullName}, but was {itemType.FullName}");
                    return new QueryShape(definition == typeof(IPage<>) ? QueryResult.Page : QueryResult.Slice, isAsync, type);
                }
            }

            return new QueryShape(type == entityType ? QueryResult.Single : QueryResult.Collection, isAsync, type);
        }
    }
}
