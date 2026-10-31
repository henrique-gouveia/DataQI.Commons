using System.Threading;
using System.Threading.Tasks;

using DataQI.Commons.Repository;
using DataQI.Commons.Repository.Paging;

namespace DataQI.Commons.Test.Repository.Sample
{
    public interface IPagedFakeRepository : ICrudRepository<FakeEntity, int>, IPagedRepository<FakeEntity, int>
    {
        ISlice<FakeEntity> FindByNameOrderByIdAsc(string name, IPageRequest page);
        ISlice<FakeEntity> FindByNameOrderByIdAsc(string name, IPageRequest page, CancellationToken cancellationToken);
        IPage<FakeEntity> FindByName(string name, IPageRequest page);
        Task<ISlice<FakeEntity>> FindByNameAsync(string name, IPageRequest page);
        Task<IPage<FakeEntity>> FindByNameAsync(string name, IPageRequest page, CancellationToken cancellationToken);
    }
}
