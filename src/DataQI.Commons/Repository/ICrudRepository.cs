using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DataQI.Commons.Query;

namespace DataQI.Commons.Repository
{
    public interface ICrudRepository<TEntity, in TId> : IDisposable where TEntity : class
    {
        void Delete(TId entity);

        Task DeleteAsync(TId entity, CancellationToken cancellationToken = default);
    
        bool Exists(TId id);

        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);

        IEnumerable<TEntity> Find(Func<ICriteria, ICriteria> criteriaBuilder);
        
        Task<IEnumerable<TEntity>> FindAsync(Func<ICriteria, ICriteria> criteriaBuilder,
            CancellationToken cancellationToken = default);

        IEnumerable<TEntity> FindAll();

        Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken = default);

        TEntity FindOne(TId id);

        Task<TEntity> FindOneAsync(TId id, CancellationToken cancellationToken = default);

        void Insert(TEntity entity);

        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        void Save(TEntity entity);

        Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}