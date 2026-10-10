namespace DataQI.Commons.Repository.Core
{
    /// <summary>
    /// Specifies how <see cref="RepositoryProxy{TRepository}"/> routes a call made on a repository interface.
    /// </summary>
    public enum DispatchKind
    {
        /// <summary>Forwards a call unchanged to a public implementation method with the same name, return type and parameter types.</summary>
        ExactMatch,
        /// <summary>Routes a collection query through parsed criteria to <c>Find</c>.</summary>
        SyncCollection,
        /// <summary>Routes an asynchronous collection query through parsed criteria to <c>FindAsync</c>.</summary>
        AsyncCollection,
        /// <summary>Routes a single entity query through parsed criteria to <c>FindOne</c>.</summary>
        SyncSingle,
        /// <summary>Routes an asynchronous single entity query through parsed criteria to <c>FindOneAsync</c>.</summary>
        AsyncSingle,
        /// <summary>Identifies a call with no matching implementation method whose invocation throws <see cref="System.Reflection.TargetInvocationException"/>.</summary>
        Unresolvable
    }
}
