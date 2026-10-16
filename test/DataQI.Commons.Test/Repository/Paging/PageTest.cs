using System;

using Xunit;

using DataQI.Commons.Repository.Paging;

namespace DataQI.Commons.Test.Repository.Paging
{
    public class PageTest
    {
        private static int[] Items(int count) => new int[count];

        [Theory]
        [InlineData(25, 10, 3)]
        [InlineData(20, 10, 2)]
        [InlineData(0, 10, 0)]
        [InlineData(1, 10, 1)]
        [InlineData(2147483648L, 1, 2147483648L)]
        [InlineData(long.MaxValue, 2, 4611686018427387904L)]
        [InlineData(long.MaxValue, 1, long.MaxValue)]
        public void TestTotalPagesUsesIntegerCeiling(long total, int size, long expectedPages)
        {
            var page = new Page<int>(Items(0), 0, size, total);

            Assert.Equal(expectedPages, page.TotalPages);
        }

        [Fact]
        public void TestFirstPageOfThreeHasNext()
        {
            var page = new Page<int>(Items(10), 0, 10, 25);

            Assert.True(page.HasNext);
            Assert.False(page.HasPrevious);
            Assert.Equal(25, page.TotalCount);
        }

        [Fact]
        public void TestLastPartialPageHasNoNext()
        {
            var page = new Page<int>(Items(5), 2, 10, 25);

            Assert.False(page.HasNext);
            Assert.True(page.HasPrevious);
        }

        [Fact]
        public void TestExactMultipleLastPageHasNoNext()
        {
            var page = new Page<int>(Items(10), 1, 10, 20);

            Assert.False(page.HasNext);
        }

        [Fact]
        public void TestPageBeyondTheEndKeepsTheRealTotal()
        {
            var page = new Page<int>(Items(0), 5, 10, 25);

            Assert.Empty(page.Items);
            Assert.Equal(25, page.TotalCount);
            Assert.Equal(3L, page.TotalPages);
            Assert.False(page.HasNext);
        }

        [Fact]
        public void TestRejectsNegativeTotal()
        {
            Assert.Throws<ArgumentException>(() => new Page<int>(Items(0), 0, 10, -1));
        }

        [Fact]
        public void TestRejectsMoreItemsThanPageSize()
        {
            Assert.Throws<ArgumentException>(() => new Page<int>(Items(3), 0, 2, 3));
        }

        [Fact]
        public void TestItemsAreACopyProtectedAgainstWrites()
        {
            var source = new System.Collections.Generic.List<int> { 1, 2 };
            var page = new Page<int>(source, 0, 2, 5);

            source.Add(3);

            Assert.Equal(2, page.Items.Count);
            Assert.True(page.HasNext);
            var list = Assert.IsAssignableFrom<System.Collections.Generic.IList<int>>(page.Items);
            Assert.Throws<NotSupportedException>(() => list.Add(4));
        }

        [Fact]
        public void TestIsUsableAsSlice()
        {
            ISlice<int> slice = new Page<int>(Items(1), 0, 1, 1);

            Assert.False(slice.HasNext);
        }
    }
}
