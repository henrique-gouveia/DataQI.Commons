using System;
using System.Collections.Generic;

using DataQI.Commons.Query;
using DataQI.Commons.Util;


namespace DataQI.Commons.Repository.Query
{
    public sealed class QueryPlan
    {
        private readonly Func<object[], ICriterion> predicateFactory;

        internal QueryPlan(Func<object[], ICriterion> predicateFactory, IReadOnlyList<IOrderCriterion> orders)
        {
            this.predicateFactory = predicateFactory;
            Orders = orders;
        }

        public IReadOnlyList<IOrderCriterion> Orders { get; }

        public void ApplyTo(ICriteria criteria, object[] values)
        {
            Assert.NotNull(criteria, "Criteria must be not null");

            criteria.Add(BuildPredicate(values));
            foreach (var order in Orders)
                criteria.AddOrder(order);
        }

        public ICriterion BuildPredicate(object[] values)
        {
            Assert.NotNull(values, "Query Values must not be null");
            return predicateFactory(values);
        }
    }
}
