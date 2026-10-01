using System;
using System.Linq;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;

using Xunit;

namespace DataQI.Commons.Test.Query.Support
{
    public class CriteriaTest
    {
        [Fact]
        public void TestAddRejectsNullCriterion()
        {
            var exception = Assert.Throws<ArgumentException>(() => new Criteria().Add(null));

            Assert.Equal("Criterion must not be null", exception.GetBaseException().Message);
        }

        [Fact]
        public void TestAddAppendsCriterionsInCallSequence()
        {
            var first = Restrictions.Null("Email");
            var second = Restrictions.Null("Phone");

            var criteria = new Criteria().Add(first).Add(second);

            Assert.Same(first, criteria.Criterions.ElementAt(0));
            Assert.Same(second, criteria.Criterions.ElementAt(1));
        }

        [Fact]
        public void TestAddRejectsNull()
        {
            var criteria = new Criteria();

            var exception = Assert.Throws<ArgumentException>(() => criteria.Add(null));

            Assert.Equal("Criterion must not be null", exception.Message);
        }

        [Fact]
        public void TestAddAppendsToCriterionsInCallSequence()
        {
            var criteria = new Criteria();
            var first = Restrictions.Null("Email");
            var second = Restrictions.Null("Phone");

            criteria.Add(first).Add(second);

            Assert.Equal(2, criteria.Criterions.Count);
            Assert.Same(first, criteria.Criterions.ElementAt(0));
            Assert.Same(second, criteria.Criterions.ElementAt(1));
        }

        [Fact]
        public void TestAddReturnsSameCriteriaInstanceForChaining()
        {
            var criteria = new Criteria();
            var result = criteria.Add(Restrictions.Null("Email"));
            Assert.Same(criteria, result);
        }

        [Fact]
        public void TestAddOrderRejectsNull()
        {
            var criteria = new Criteria();

            var exception = Assert.Throws<ArgumentException>(() => criteria.AddOrder(null));

            Assert.Equal("Order must not be null", exception.Message);
        }

        [Fact]
        public void TestCriterionsAndOrdersAreExposedByTheInterface()
        {
            ICriteria criteria = new Criteria()
                .Add(Restrictions.Null("Email"))
                .AddOrder(Order.Asc("Name"));

            Assert.Single(criteria.Criterions);
            Assert.Single(criteria.Orders);
        }

        [Fact]
        public void TestAddOrderAppendsToOrdersInCallSequence()
        {
            var criteria = new Criteria();

            criteria.AddOrder(Order.Asc("Name")).AddOrder(Order.Desc("Stock"));

            Assert.Equal(2, criteria.Orders.Count);
            Assert.Equal("Name", criteria.Orders.ElementAt(0).GetPropertyName());
            Assert.Equal(OrderDirection.Asc, criteria.Orders.ElementAt(0).GetDirection());
            Assert.Equal("Stock", criteria.Orders.ElementAt(1).GetPropertyName());
            Assert.Equal(OrderDirection.Desc, criteria.Orders.ElementAt(1).GetDirection());
        }

        [Fact]
        public void TestAddOrderReturnsSameCriteriaInstanceForChaining()
        {
            var criteria = new Criteria();
            var result = criteria.AddOrder(Order.Asc("Name"));
            Assert.Same(criteria, result);
        }
    }
}
