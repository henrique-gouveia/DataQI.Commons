using System;
using System.Linq;

using Xunit;

using DataQI.Commons.Repository;
using DataQI.Commons.Repository.Paging;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Paging
{
    public class PagedRepositoryContractTest
    {
        [Fact]
        public void TestDeclaresTheFourPagingMethods()
        {
            var names = typeof(IPagedRepository<FakeEntity, int>).GetMethods().Select(m => m.Name).OrderBy(n => n);

            Assert.Equal(new[] { "FindPage", "FindPageAsync", "FindSlice", "FindSliceAsync" }, names);
        }

        [Fact]
        public void TestCrudAndPagingAreIndependentContractsWithTheSameMarker()
        {
            var crud = typeof(ICrudRepository<FakeEntity, int>);
            var paged = typeof(IPagedRepository<FakeEntity, int>);
            var marker = typeof(IRepository<FakeEntity, int>);

            Assert.True(marker.IsAssignableFrom(crud));
            Assert.True(marker.IsAssignableFrom(paged));
            Assert.False(crud.IsAssignableFrom(paged));
            Assert.False(paged.IsAssignableFrom(crud));
            Assert.True(typeof(IDisposable).IsAssignableFrom(crud));
            Assert.False(typeof(IDisposable).IsAssignableFrom(paged));
            Assert.Empty(marker.GetMethods());
            Assert.DoesNotContain(crud.GetMethods(),
                m => m.Name.StartsWith("FindSlice") || m.Name.StartsWith("FindPage"));
        }
    }
}
