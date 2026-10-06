using System.Collections.ObjectModel;
using System.Collections.Generic;

using DataQI.Commons.Util;

namespace DataQI.Commons.Query.Support
{
    public class Criteria : ICriteria
    {
        protected readonly IList<ICriterion> criterions = new List<ICriterion>();
        protected readonly IList<IOrderCriterion> orders = new List<IOrderCriterion>();

        public ICriteria Add(ICriterion criterion)
        {
            Assert.NotNull(criterion, "Criterion must not be null");
            criterions.Add(criterion);
            return this;
        }

        public ICriteria AddOrder(IOrderCriterion order)
        {
            Assert.NotNull(order, "Order must not be null");
            orders.Add(order);
            return this;
        }

        public IReadOnlyCollection<ICriterion> Criterions => new ReadOnlyCollection<ICriterion>(criterions);
        public IReadOnlyCollection<IOrderCriterion> Orders => new ReadOnlyCollection<IOrderCriterion>(orders);
    }
}
