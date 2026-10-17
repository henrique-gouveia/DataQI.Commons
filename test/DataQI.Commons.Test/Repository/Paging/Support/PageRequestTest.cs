using System;
using System.Linq;

using Xunit;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Repository.Paging;
using DataQI.Commons.Repository.Paging.Support;

namespace DataQI.Commons.Test.Repository.Paging.Support
{
    public class PageRequestTest
    {
        [Fact]
        public void TestOfExposesIndexSizeAndOffset()
        {
            var request = PageRequest.Of(2, 10);

            Assert.Equal(2, request.PageIndex);
            Assert.Equal(10, request.PageSize);
            Assert.Equal(20L, request.Offset);
            Assert.Empty(request.Orders);
        }

        [Fact]
        public void TestOfKeepsOrdersInGivenOrder()
        {
            var request = PageRequest.Of(0, 5, Order.Desc("Name"), Order.Asc("Id"));

            Assert.Equal(new[] { "Name", "Id" }, request.Orders.Select(o => o.PropertyName));
        }

        [Theory]
        [InlineData(-1, 10)]
        [InlineData(0, 0)]
        [InlineData(0, -5)]
        [InlineData(0, int.MaxValue)]
        public void TestOfRejectsInvalidIndexOrSize(int pageIndex, int pageSize)
        {
            Assert.Throws<ArgumentException>(() => PageRequest.Of(pageIndex, pageSize));
        }

        [Fact]
        public void TestOfRejectsNullOrder()
        {
            Assert.Throws<ArgumentException>(() => PageRequest.Of(0, 10, [null]));
        }

        [Fact]
        public void TestOffsetDoesNotOverflowInt()
        {
            var request = PageRequest.Of(int.MaxValue, 10);

            Assert.Equal((long)int.MaxValue * 10, request.Offset);
        }

        [Fact]
        public void TestOrdersAreCopiedSoMutableCriteriaCannotChangeTheRequest()
        {
            var mutable = new MutableOrder { PropertyName = "Name", Direction = OrderDirection.Asc };
            var request = PageRequest.Of(0, 10, mutable);

            mutable.PropertyName = "Other";
            mutable.Direction = OrderDirection.Desc;

            Assert.Equal("Name", request.Orders[0].PropertyName);
            Assert.Equal(OrderDirection.Asc, request.Orders[0].Direction);
        }

        [Fact]
        public void TestOrdersCannotBeModifiedThroughACast()
        {
            var request = PageRequest.Of(0, 10, Order.Asc("Name"));

            Assert.False(request.Orders is IOrderCriterion[]);
            var list = Assert.IsAssignableFrom<System.Collections.Generic.IList<IOrderCriterion>>(request.Orders);
            Assert.Throws<NotSupportedException>(() => list.Add(Order.Asc("Other")));
            Assert.Throws<NotSupportedException>(() => list[0] = Order.Desc("Other"));
        }

        [Fact]
        public void TestOrdersAreNotAffectedByLaterChangesToTheSourceArray()
        {
            var orders = new[] { Order.Asc("Name") };
            var request = PageRequest.Of(0, 10, orders);

            orders[0] = Order.Desc("Other");

            Assert.Equal("Name", request.Orders.Single().PropertyName);
        }

        private sealed class MutableOrder : IOrderCriterion
        {
            public string PropertyName { get; set; }
            public OrderDirection Direction { get; set; }
        }
    }
}
