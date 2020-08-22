using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DataQI.Commons.Query;

namespace DataQI.Commons.Repository
{
    public interface ICrudRepository<TEntity, TId> : IDisposable where TEntity : class, new()
    {
        void Delete(TId entity);

        Task DeleteAsync(TId entity);
    
        bool Exists(TId id);

        Task<bool> ExistsAsync(TId id);

        IEnumerable<TEntity> Find(Func<ICriteria, ICriteria> criteriaBuilder);
        
        Task<IEnumerable<TEntity>> FindAsync(Func<ICriteria, ICriteria> criteriaBuilder);

        IEnumerable<TEntity> FindAll();

        Task<IEnumerable<TEntity>> FindAllAsync();

        TEntity FindOne(TId id);

        Task<TEntity> FindOneAsync(TId id);

        void Insert(TEntity entity);

        Task InsertAsync(TEntity entity);

        void Save(TEntity entity);

        Task SaveAsync(TEntity entity);
    }
}