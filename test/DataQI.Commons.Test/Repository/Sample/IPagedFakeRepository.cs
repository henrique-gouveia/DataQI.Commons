using System.Threading;
using System.Threading.Tasks;

using DataQI.Commons.Repository.Paging;

namespace DataQI.Commons.Test.Repository.Sample
{
    public interface IPagedFakeRepository : IPagedRepository<FakeEntity, int>
    {
        ISlice<FakeEntity> FindByNameOrderByIdAsc(string name, IPageRequest page);
        IPage<FakeEntity> FindByName(string name, IPageRequest page);
        Task<ISlice<FakeEntity>> FindByNameAsync(string name, IPageRequest page);
        Task<IPage<FakeEntity>> FindByNameAsync(string name, IPageRequest page, CancellationToken cancellationToken);
    }
}
