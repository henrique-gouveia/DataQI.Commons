using DataQI.Commons.Repository.Paging;

namespace DataQI.Commons.Test.Repository.Sample
{
    public interface IPagedFakeRepository : IPagedRepository<FakeEntity, int>
    {
    }
}
