using System;
using System.Collections.Generic;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Ast;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Repository.Query;

using Xunit;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryPlanTest
    {
        [Fact]
        public void TestBuildPredicateUsesTheSuppliedValues()
        {
            var plan = new QueryPlan(
                values => Restrictions.Equal("FirstName", values[0]),
                new List<IOrderCriterion>());

            var comparison = Assert.IsType<Comparison>(plan.BuildPredicate(new object[] { "Adams" }));

            Assert.Equal("Adams", comparison.Value);
        }

        [Fact]
        public void TestBuildPredicateRejectsNullValues()
        {
            var plan = new QueryPlan(values => Restrictions.Null("Email"), new List<IOrderCriterion>());

            var exception = Assert.Throws<ArgumentException>(() => plan.BuildPredicate(null));

            Assert.Equal("Query Values must not be null", exception.GetBaseException().Message);
        }

        [Fact]
        public void TestOrdersAreExposedInTheirGivenSequence()
        {
            var orders = new List<IOrderCriterion> { Order.Asc("Name"), Order.Desc("Stock") };
            var plan = new QueryPlan(values => Restrictions.Null("Email"), orders);

            Assert.Equal(2, plan.Orders.Count);
            Assert.Equal("Name", plan.Orders[0].GetPropertyName());
            Assert.Equal("Stock", plan.Orders[1].GetPropertyName());
        }
    }
}
