using System;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

using DataQI.Commons.Query;
using DataQI.Commons.Repository.Core;
using DataQI.Commons.Repository.Paging;
using DataQI.Commons.Repository.Paging.Support;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Core
{
    public class RepositoryProxyImplementationResolutionTest
    {
        private static IPagedFakeRepository CreateRepository()
            => RepositoryProxy<IPagedFakeRepository>.Create(() => new ShadowingImplementation());

        [Fact]
        public void TestOpenGenericMethodDoesNotHideTheInheritedImplementation()
        {
            var result = CreateRepository().FindByNameOrderByIdAsc("ana", PageRequest.Of(0, 10));

            Assert.False(result.HasNext);
        }

        [Fact]
        public void TestStaticMethodDoesNotHideTheInstanceImplementation()
        {
            var result = CreateRepository().FindByName("ana", PageRequest.Of(0, 10));

            Assert.Equal(0, result.TotalCount);
        }

        public class PagedImplementation
        {
            public ISlice<FakeEntity> FindSlice(Func<ICriteria, ICriteria> criteria, IPageRequest page)
                => new Slice<FakeEntity>([], page.PageIndex, page.PageSize, false);

            public Task<ISlice<FakeEntity>> FindSliceAsync(Func<ICriteria, ICriteria> criteria, IPageRequest page, CancellationToken cancellationToken)
                => Task.FromResult<ISlice<FakeEntity>>(new Slice<FakeEntity>([], page.PageIndex, page.PageSize, false));

            public IPage<FakeEntity> FindPage(Func<ICriteria, ICriteria> criteria, IPageRequest page)
                => new Page<FakeEntity>([], page.PageIndex, page.PageSize, 0);

            public Task<IPage<FakeEntity>> FindPageAsync(Func<ICriteria, ICriteria> criteria, IPageRequest page, CancellationToken cancellationToken)
                => Task.FromResult<IPage<FakeEntity>>(new Page<FakeEntity>([], page.PageIndex, page.PageSize, 0));
        }

        public class ShadowingImplementation : PagedImplementation
        {
            public ISlice<T> FindSlice<T>(Func<ICriteria, ICriteria> criteria, IPageRequest page)
                => throw new InvalidOperationException("an open generic method must not be chosen");

            public static IPage<FakeEntity> FindPage(Func<ICriteria, ICriteria> criteria, IPageRequest page)
                => throw new InvalidOperationException("a static method must not be chosen");
        }
    }
}
