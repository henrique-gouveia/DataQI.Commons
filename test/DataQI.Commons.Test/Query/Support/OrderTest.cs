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
            Assert.Equal("Name", criterion.GetPropertyName());
            Assert.Equal(OrderDirection.Asc, criterion.GetDirection());
        }

        [Fact]
        public void TestDescBuildsDescendingOrderCriterion()
        {
            var criterion = Order.Desc("Stock");
            Assert.Equal("Stock", criterion.GetPropertyName());
            Assert.Equal(OrderDirection.Desc, criterion.GetDirection());
        }
    }
}
