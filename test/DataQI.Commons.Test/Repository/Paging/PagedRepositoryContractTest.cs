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
        public void TestExtendsTheCrudRepositoryWithoutChangingIt()
        {
            Assert.True(typeof(ICrudRepository<FakeEntity, int>).IsAssignableFrom(
                typeof(IPagedRepository<FakeEntity, int>)));
            Assert.DoesNotContain(typeof(ICrudRepository<FakeEntity, int>).GetMethods(),
                m => m.Name.StartsWith("FindSlice") || m.Name.StartsWith("FindPage"));
        }
    }
}
