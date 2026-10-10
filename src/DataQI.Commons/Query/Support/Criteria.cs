using System.Collections.Generic;
using System.Collections.ObjectModel;

using DataQI.Commons.Util;

namespace DataQI.Commons.Query.Support
{
    /// <summary>Provides an <see cref="ICriteria"/> implementation that keeps criterions and orders in lists.</summary>
    public class Criteria : ICriteria
    {
        /// <summary>Stores the criterions added so far, in insertion order.</summary>
        protected readonly IList<ICriterion> criterions = new List<ICriterion>();
        /// <summary>Stores the orderings added so far, in insertion order.</summary>
        protected readonly IList<IOrderCriterion> orders = new List<IOrderCriterion>();

        /// <summary>Adds a criterion to the query.</summary>
        /// <param name="criterion">The criterion to add; must not be <c>null</c>.</param>
        /// <returns>This instance, to allow chaining.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="criterion"/> is <c>null</c>.</exception>
        public ICriteria Add(ICriterion criterion)
        {
            Assert.NotNull(criterion, "Criterion must not be null");
            criterions.Add(criterion);
            return this;
        }

        /// <summary>Adds an ordering to the query.</summary>
        /// <param name="order">The ordering to add; must not be <c>null</c>.</param>
        /// <returns>This instance, to allow chaining.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="order"/> is <c>null</c>.</exception>
        public ICriteria AddOrder(IOrderCriterion order)
        {
            Assert.NotNull(order, "Order must not be null");
            orders.Add(order);
            return this;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ICriterion> Criterions => new ReadOnlyCollection<ICriterion>(criterions);
        /// <inheritdoc />
        public IReadOnlyCollection<IOrderCriterion> Orders => new ReadOnlyCollection<IOrderCriterion>(orders);
    }
}
