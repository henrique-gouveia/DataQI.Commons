using System;
using System.Linq;

using DataQI.Commons.Query.Support;

using Xunit;

namespace DataQI.Commons.Test.Query.Support
{
    public class CriteriaTest
    {
        [Fact]
        public void TestAddOrderRejectsNull()
        {
            var criteria = new Criteria();

            var exception = Assert.Throws<ArgumentException>(() => criteria.AddOrder(null));

            Assert.Equal("Order must not be null", exception.Message);
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
