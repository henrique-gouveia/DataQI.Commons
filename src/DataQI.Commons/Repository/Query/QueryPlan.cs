using System;
using System.Collections.Generic;

using DataQI.Commons.Query;
using DataQI.Commons.Util;

using Ast = DataQI.Commons.Query.Ast;

namespace DataQI.Commons.Repository.Query
{
    public sealed class QueryPlan
    {
        private readonly Func<object[], Ast.ICriterion> predicateFactory;

        internal QueryPlan(Func<object[], Ast.ICriterion> predicateFactory, IReadOnlyList<IOrderCriterion> orders)
        {
            this.predicateFactory = predicateFactory;
            Orders = orders;
        }

        public IReadOnlyList<IOrderCriterion> Orders { get; }

        public Ast.ICriterion BuildPredicate(object[] values)
        {
            Assert.NotNull(values, "Query Values must not be null");
            return predicateFactory(values);
        }
    }
}
