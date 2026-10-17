using System;
using System.Linq;
using Xunit;
using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Repository.Paging.Support;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Paging.Support
{
    public class PagingRulesTest
    {
        [Fact]
        public void TestMergeKeepsMethodThenCriteriaThenPageThenKeyOrderAndDropsRepeats()
        {
            var orders = PagingRules.MergeOrders<FakeEntity>(
                new[] { Order.Asc("Name") },
                PageRequest.Of(0, 10, Order.Desc("Name"), Order.Desc("Price")),
                new[] { "Id" });

            Assert.Equal(new[] { "Name", "Price", "Id" }, orders.Select(o => o.PropertyName));
            Assert.Equal(OrderDirection.Asc, orders[0].Direction);
            Assert.Equal(OrderDirection.Asc, orders[2].Direction);
        }

        [Fact]
        public void TestMergeDoesNotRepeatAKeyAlreadyInTheOrdering()
        {
            var orders = PagingRules.MergeOrders<FakeEntity>(
                new[] { Order.Desc("Id") }, PageRequest.Of(0, 10), new[] { "Id" });

            Assert.Single(orders);
            Assert.Equal(OrderDirection.Desc, orders[0].Direction);
        }

        [Fact]
        public void TestMergeAppendsEveryPartOfACompositeKey()
        {
            var orders = PagingRules.MergeOrders<FakeEntity>(
                [], PageRequest.Of(0, 10), new[] { "A", "B" });

            Assert.Equal(new[] { "A", "B" }, orders.Select(o => o.PropertyName));
        }

        [Fact]
        public void TestMergeFailsWithoutKeyAndWithoutExplicitOrder()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                PagingRules.MergeOrders<FakeEntity>([], PageRequest.Of(0, 10), []));

            Assert.Contains(nameof(FakeEntity), exception.Message);
        }

        [Fact]
        public void TestMergeAcceptsAnEntityWithoutKeyWhenAnOrderIsGiven()
        {
            var orders = PagingRules.MergeOrders<FakeEntity>(
                new[] { Order.Asc("Name") }, PageRequest.Of(0, 10), []);

            Assert.Single(orders);
        }

        [Fact]
        public void TestEnsurePageRejectsNull()
        {
            Assert.Throws<ArgumentException>(() => PagingRules.EnsurePage(null));
        }

        [Fact]
        public void TestEnsurePageRejectsOffsetBeyondIntMax()
        {
            var page = PageRequest.Of(int.MaxValue, 10);

            Assert.Throws<ArgumentException>(() => PagingRules.EnsurePage(page));
        }

        [Fact]
        public void TestEnsurePageAcceptsAValidCustomImplementation()
        {
            PagingRules.EnsurePage(new CustomPageRequest(2, 10, 20, []));
        }

        [Theory]
        [InlineData(-1, 10, -10L)]
        [InlineData(0, 0, 0L)]
        [InlineData(0, -3, 0L)]
        [InlineData(0, int.MaxValue, 0L)]
        [InlineData(2, 10, 5L)]
        [InlineData(2, 10, -20L)]
        public void TestEnsurePageRejectsInconsistentCustomImplementations(int index, int size, long offset)
        {
            var page = new CustomPageRequest(index, size, offset, []);

            Assert.Throws<ArgumentException>(() => PagingRules.EnsurePage(page));
        }

        [Fact]
        public void TestEnsurePageRejectsNullOrders()
        {
            var page = new CustomPageRequest(0, 10, 0, null);

            Assert.Throws<ArgumentException>(() => PagingRules.EnsurePage(page));
        }

        [Fact]
        public void TestEnsurePageRejectsNullOrderOrEmptyPropertyName()
        {
            Assert.Throws<ArgumentException>(() => PagingRules.EnsurePage(
                new CustomPageRequest(0, 10, 0, new IOrderCriterion[] { null })));
            Assert.Throws<ArgumentException>(() => PagingRules.EnsurePage(
                new CustomPageRequest(0, 10, 0, new[] { Order.Asc("") })));
        }

        [Theory]
        [InlineData(null, OrderDirection.Asc)]
        [InlineData("", OrderDirection.Asc)]
        [InlineData("   ", OrderDirection.Asc)]
        [InlineData("Name", (OrderDirection)123)]
        public void TestEnsurePageRejectsInvalidCustomOrders(string name, OrderDirection direction)
        {
            var page = new CustomPageRequest(0, 10, 0, new IOrderCriterion[] { new CustomOrder(name, direction) });
            Assert.Throws<ArgumentException>(() => PagingRules.EnsurePage(page));
        }

        [Theory]
        [InlineData(5, 10, 2, 25L)]
        [InlineData(0, 10, 0, 0L)]
        public void TestDeducesTheTotalWhenThePageRevealsIt(int items, int size, int index, long expected)
        {
            Assert.Equal(expected, PagingRules.DeduceTotal(items, PageRequest.Of(index, size)));
        }

        [Theory]
        [InlineData(10, 10, 0)]
        [InlineData(0, 10, 5)]
        public void TestDoesNotDeduceTheTotalOtherwise(int items, int size, int index)
        {
            Assert.Null(PagingRules.DeduceTotal(items, PageRequest.Of(index, size)));
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(10, false)]
        [InlineData(11, true)]
        public void TestSliceTrimsTheExtraRowWithoutChangingTheSource(int count, bool hasNext)
        {
            var source = Enumerable.Range(0, count).ToList();
            var slice = PagingRules.ToSlice(source, PageRequest.Of(1, 10));

            Assert.Equal(Math.Min(count, 10), slice.Items.Count);
            Assert.Equal(hasNext, slice.HasNext);
            Assert.True(slice.HasPrevious);
            Assert.Equal(count, source.Count);
            Assert.Equal(source.Take(10), slice.Items);
        }

        [Fact]
        public void TestSliceRejectsNullRequestAndNullRows()
        {
            Assert.Throws<ArgumentException>(() => PagingRules.ToSlice(Array.Empty<int>(), null));
            Assert.Throws<ArgumentException>(() => PagingRules.ToSlice<int>(null, PageRequest.Of(0, 10)));
        }

        private sealed class CustomOrder : IOrderCriterion
        {
            public CustomOrder(string name, OrderDirection direction)
            {
                PropertyName = name;
                Direction = direction;
            }
            public string PropertyName { get; }
            public OrderDirection Direction { get; }
        }
    }
}
