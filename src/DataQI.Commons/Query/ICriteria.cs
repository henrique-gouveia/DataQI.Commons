using System.Collections.Generic;

namespace DataQI.Commons.Query
{
    /// <summary>
    /// Collects the criterions and the ordering that make up a query.
    /// </summary>
    /// <remarks>
    /// Repositories pass an empty instance to the <c>Func&lt;ICriteria, ICriteria&gt;</c> builder given to
    /// <c>Find</c> and <c>FindOne</c>. The providers combine the top level criterions with a logical AND.
    /// </remarks>
    public interface ICriteria
    {
        /// <summary>Adds a criterion to the query.</summary>
        /// <param name="criterion">The criterion to add; must not be <c>null</c>.</param>
        /// <returns>This instance, to allow chaining.</returns>
        ICriteria Add(ICriterion criterion);
        /// <summary>Adds an ordering to the query to be applied in insertion order.</summary>
        /// <param name="order">The ordering to add; must not be <c>null</c>.</param>
        /// <returns>This instance, to allow chaining.</returns>
        ICriteria AddOrder(IOrderCriterion order);
        /// <summary>Gets the criterions added so far, in insertion order.</summary>
        IReadOnlyCollection<ICriterion> Criterions { get; }
        /// <summary>Gets the orderings added so far, in insertion order.</summary>
        IReadOnlyCollection<IOrderCriterion> Orders { get; }
    }
}
