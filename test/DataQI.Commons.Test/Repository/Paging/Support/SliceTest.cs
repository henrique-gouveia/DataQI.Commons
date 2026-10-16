using System;

using Xunit;

using DataQI.Commons.Repository.Paging;
using DataQI.Commons.Repository.Paging.Support;

namespace DataQI.Commons.Test.Repository.Paging.Support
{
    public class SliceTest
    {
        [Fact]
        public void TestExposesItsValues()
        {
            var slice = new Slice<string>(new[] { "a", "b" }, 1, 2, true);

            Assert.Equal(new[] { "a", "b" }, slice.Items);
            Assert.Equal(1, slice.PageIndex);
            Assert.Equal(2, slice.PageSize);
            Assert.True(slice.HasNext);
            Assert.True(slice.HasPrevious);
        }

        [Fact]
        public void TestFirstPageHasNoPrevious()
        {
            var slice = new Slice<string>([], 0, 10, false);

            Assert.False(slice.HasPrevious);
        }

        [Fact]
        public void TestRejectsMoreItemsThanPageSize()
        {
            Assert.Throws<ArgumentException>(() =>
                new Slice<int>(new[] { 1, 2, 3 }, 0, 2, false));
        }

        [Fact]
        public void TestRejectsNullItems()
        {
            Assert.Throws<ArgumentException>(() => new Slice<int>(null, 0, 2, false));
        }

        [Theory]
        [InlineData(-1, 10)]
        [InlineData(0, 0)]
        public void TestRejectsInvalidIndexOrSize(int pageIndex, int pageSize)
        {
            Assert.Throws<ArgumentException>(() => new Slice<int>([], pageIndex, pageSize, false));
        }

        [Fact]
        public void TestItemsAreACopyProtectedAgainstWrites()
        {
            var source = new System.Collections.Generic.List<int> { 1, 2 };
            var slice = new Slice<int>(source, 0, 2, false);

            source.Add(3);
            source[0] = 99;

            Assert.Equal(new[] { 1, 2 }, slice.Items);
            var list = Assert.IsAssignableFrom<System.Collections.Generic.IList<int>>(slice.Items);
            Assert.Throws<NotSupportedException>(() => list.Add(4));
            Assert.Throws<NotSupportedException>(() => list[0] = 5);
        }

        [Fact]
        public void TestIsCovariant()
        {
            ISlice<string> slice = new Slice<string>(new[] { "a" }, 0, 1, false);

            ISlice<object> asObject = slice;

            Assert.Single(asObject.Items);
        }
    }
}
