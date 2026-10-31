using System;
using System.Threading;
using System.Threading.Tasks;

using DataQI.Commons.Query;

namespace DataQI.Commons.Repository.Paging
{
    /// <summary>
    /// Defines paging independently of CRUD and disposal. Fetches pages using criteria. A null page throws ArgumentException; c =&gt; c selects all rows.
    /// Ordering is completed with primary keys for stable pages.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The identifier type.</typeparam>
    public interface IPagedRepository<TEntity, in TId> : IRepository<TEntity, TId> where TEntity : class
    {
        /// <summary>Fetches a slice without counting matching rows.</summary>
        ISlice<TEntity> FindSlice(Func<ICriteria, ICriteria> criteriaBuilder, IPageRequest page);
        /// <summary>Asynchronously fetches a slice without counting matching rows.</summary>
        Task<ISlice<TEntity>> FindSliceAsync(Func<ICriteria, ICriteria> criteriaBuilder, IPageRequest page, CancellationToken cancellationToken = default);
        /// <summary>Fetches a page and its total count.</summary>
        IPage<TEntity> FindPage(Func<ICriteria, ICriteria> criteriaBuilder, IPageRequest page);
        /// <summary>Asynchronously fetches a page and its total count.</summary>
        Task<IPage<TEntity>> FindPageAsync(Func<ICriteria, ICriteria> criteriaBuilder, IPageRequest page, CancellationToken cancellationToken = default);
    }
}
