using System.Collections.Generic;
using System.Collections.ObjectModel;

using DataQI.Commons.Util;

namespace DataQI.Commons.Query.Support
{
    public class Criteria : ICriteria
    {
        protected readonly IList<Ast.ICriterion> criterions = new List<Ast.ICriterion>();
        protected readonly IList<IOrderCriterion> orders = new List<IOrderCriterion>();

        public ICriteria Add(Ast.ICriterion criterion)
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

        public IReadOnlyCollection<Ast.ICriterion> Criterions => new ReadOnlyCollection<Ast.ICriterion>(criterions);
        public IReadOnlyCollection<IOrderCriterion> Orders => new ReadOnlyCollection<IOrderCriterion>(orders);
    }
}
