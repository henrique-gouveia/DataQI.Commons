using DataQI.Commons.Query.Support;

using Xunit;

namespace DataQI.Commons.Test.Query.Support
{
    public class OrderTest
    {
        [Fact]
        public void TestAscBuildsAscendingOrderCriterion()
        {
            var criterion = Order.Asc("Name");
            Assert.Equal("Name", criterion.PropertyName);
            Assert.Equal(OrderDirection.Asc, criterion.Direction);
        }

        [Fact]
        public void TestDescBuildsDescendingOrderCriterion()
        {
            var criterion = Order.Desc("Stock");
            Assert.Equal("Stock", criterion.PropertyName);
            Assert.Equal(OrderDirection.Desc, criterion.Direction);
        }
    }
}
