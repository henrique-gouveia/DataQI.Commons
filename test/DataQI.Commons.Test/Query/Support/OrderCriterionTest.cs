using DataQI.Commons.Query.Support;

using Xunit;

namespace DataQI.Commons.Test.Query.Support
{
    public class OrderCriterionTest
    {
        [Fact]
        public void TestGetPropertyNameReturnsConstructorValue()
        {
            var criterion = new OrderCriterion("Name", OrderDirection.Asc);
            Assert.Equal("Name", criterion.GetPropertyName());
        }

        [Fact]
        public void TestGetDirectionReturnsConstructorValue()
        {
            var criterion = new OrderCriterion("Stock", OrderDirection.Desc);
            Assert.Equal(OrderDirection.Desc, criterion.GetDirection());
        }
    }
}
