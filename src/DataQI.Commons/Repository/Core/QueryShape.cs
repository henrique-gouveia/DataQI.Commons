using System;
using System.Linq;
using System.Reflection;
using System.Threading;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Query;
using DataQI.Commons.Repository.Paging;
using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Core
{
    /// <summary>Describes the form of a query method: what it returns and whether it is asynchronous.</summary>
    public sealed class QueryShape
    {
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
                    case QueryResult.Collection:
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
                    case QueryResult.Collection:
                    default:
                        return string.Empty;
                }
            }
        }

        private QueryShape(QueryResult result, bool isAsync, Type resultType)
        {
            Result = result;
            IsAsync = isAsync;
            ResultType = resultType;
        }

        /// <summary>Classifies a query method of a repository interface.</summary>
        /// <exception cref="InvalidOperationException">A paged result is not of the repository entity.</exception>
        public static QueryShape Classify(MethodInfo method, Type entityType)
        {
            var type = method.ReturnType;
            var isAsync = type.TryGetAsyncResultType(out var inner);
            if (isAsync) type = inner;

            if (!type.IsGenericType)
                return new QueryShape(type == entityType ? QueryResult.Single : QueryResult.Collection, isAsync, type);

            var definition = type.GetGenericTypeDefinition();
            if (definition != typeof(ISlice<>) && definition != typeof(IPage<>))
                return new QueryShape(type == entityType ? QueryResult.Single : QueryResult.Collection, isAsync, type);

            var itemType = type.GetGenericArguments()[0];
            Assert.True<InvalidOperationException>(itemType == entityType,
                $"{method.DeclaringType?.Name}.{method.Name}: paging result T must be the repository entity {entityType.FullName}, but was {itemType.FullName}");

            return new QueryShape(definition == typeof(IPage<>) ? QueryResult.Page : QueryResult.Slice, isAsync, type);
        }

        /// <summary>Identifies what a query method returns.</summary>
        public enum QueryResult
        {
            /// <summary>Returns a collection of entities.</summary>
            Collection,
            /// <summary>Returns a single entity.</summary>
            Single,
            /// <summary>Returns an <see cref="ISlice{T}"/>.</summary>
            Slice,
            /// <summary>Returns an <see cref="IPage{T}"/>.</summary>
            Page
        }

        /// <summary>Builds the arguments for the resolved implementation method.</summary>
        /// <param name="criteriaBuilder">The criteria builder made from the query method arguments.</param>
        /// <param name="page">The page request, when the query is paged.</param>
        /// <param name="cancellationToken">The cancellation token for an asynchronous query.</param>
        /// <returns>The arguments to pass to the implementation method.</returns>
        public object[] BuildImplementationArguments(
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

        /// <summary>Builds the query argument values from a repository interface call.</summary>
        /// <param name="args">The arguments passed to the query method.</param>
        /// <param name="hasCancellationToken">Whether the last argument is a cancellation token.</param>
        /// <returns>The criteria values, page request and cancellation token.</returns>
        public QueryMethodArguments BuildQueryMethodArguments(object[] args, bool hasCancellationToken)
        {
            var withoutToken = hasCancellationToken ? args.Take(args.Length - 1).ToArray() : args;
            var cancellationToken = hasCancellationToken ? (CancellationToken)args[args.Length - 1] : default;
            var criteriaArgs = HasPageRequest ? withoutToken.Take(withoutToken.Length - 1).ToArray() : withoutToken;
            var page = HasPageRequest ? (IPageRequest)withoutToken[withoutToken.Length - 1] : null;
            return new QueryMethodArguments(criteriaArgs, page, cancellationToken);
        }

        /// <summary>Contains the arguments extracted from a query method call.</summary>
        public readonly struct QueryMethodArguments
        {
            internal QueryMethodArguments(object[] criteriaArgs, IPageRequest page, CancellationToken cancellationToken)
            {
                CriteriaArgs = criteriaArgs;
                Page = page;
                CancellationToken = cancellationToken;
            }

            /// <summary>Gets the arguments used to build query criteria.</summary>
            public object[] CriteriaArgs { get; }
            /// <summary>Gets the page request, or <c>null</c> for a non-paged query.</summary>
            public IPageRequest Page { get; }
            /// <summary>Gets the cancellation token for the query method.</summary>
            public CancellationToken CancellationToken { get; }
        }
    }
}
