using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Net.Data.Commons.Criterions;

namespace Net.Data.Commons.Repository
{
    public interface ICrudRepository<TEntity, TId> where TEntity : class, new()
    {
        void Delete(TId entity);

        Task DeleteAsync(TId entity);
    
         bool Exists(TId id);

        Task<bool> ExistsAsync(TId id);

        IEnumerable<TEntity> Find(ICriteria criteria);
        
        Task<IEnumerable<TEntity>> FindAsync(ICriteria criteria);

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