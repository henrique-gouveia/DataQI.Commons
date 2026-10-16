using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DataQI.Commons.Repository.Paging;

namespace DataQI.Commons.Test.Repository.Sample
{
    public interface IPagedWithoutRequestRepository : IPagedRepository<FakeEntity, int>
    {
        ISlice<FakeEntity> FindByName(string name);
    }

    public interface IRequestWithPlainReturnRepository : IPagedRepository<FakeEntity, int>
    {
        IEnumerable<FakeEntity> FindByName(string name, IPageRequest page);
    }

    public interface IDuplicatedRequestRepository : IPagedRepository<FakeEntity, int>
    {
        IPage<FakeEntity> FindByName(IPageRequest first, string name, IPageRequest second);
    }

    public interface IRequestNotLastRepository : IPagedRepository<FakeEntity, int>
    {
        ISlice<FakeEntity> FindByName(IPageRequest page, string name);
    }

    public interface IRequestBeforeTokenRepository : IPagedRepository<FakeEntity, int>
    {
        Task<ISlice<FakeEntity>> FindByNameAsync(string name, IPageRequest page, CancellationToken cancellationToken);
    }

    public interface IWrongEntityPagedRepository : IPagedRepository<FakeEntity, int>
    {
        ISlice<string> FindByName(string name, IPageRequest page);
    }

    public interface INonTrailingTokenPagedRepository : IPagedRepository<FakeEntity, int>
    {
        Task<ISlice<FakeEntity>> FindByNameAsync(CancellationToken token, string name, IPageRequest page);
    }

    public interface IDuplicatedTokenPagedRepository : IPagedRepository<FakeEntity, int>
    {
        Task<ISlice<FakeEntity>> FindByNameAsync(CancellationToken first, string name, IPageRequest page, CancellationToken last);
    }
    public interface IWrongEntityPageRepository : IPagedRepository<FakeEntity, int>
    {
        IPage<string> FindByName(string name, IPageRequest page);
    }

    public interface IWrongEntityAsyncSliceRepository : IPagedRepository<FakeEntity, int>
    {
        Task<ISlice<string>> FindByName(string name, IPageRequest page);
    }

    public interface IWrongEntityAsyncPageRepository : IPagedRepository<FakeEntity, int>
    {
        Task<IPage<string>> FindByName(string name, IPageRequest page);
    }
}
