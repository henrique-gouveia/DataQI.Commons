using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;

using Xunit;

namespace DataQI.Commons.Test.Query.Support
{
    public class OrderCriterionTest
    {
        [Fact]
        public void TestPropertyNameReturnsConstructorValue()
        {
            IOrderCriterion criterion = new OrderCriterion("Name", OrderDirection.Asc);
            Assert.Equal("Name", criterion.PropertyName);
        }

        [Fact]
        public void TestDirectionReturnsConstructorValue()
        {
            IOrderCriterion criterion = new OrderCriterion("Stock", OrderDirection.Desc);
            Assert.Equal(OrderDirection.Desc, criterion.Direction);
        }
    }
}
