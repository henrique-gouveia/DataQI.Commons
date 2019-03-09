using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Data.Commons.Repository
{
    public interface ICrudRepository<TEntity, TId> where TEntity : class, new()
    {
        IEnumerable<TEntity> FindAll();

        Task<IEnumerable<TEntity>> FindAllAsync();

        TEntity FindOne(TId id);

        Task<TEntity> FindOneAsync(TId id);

        bool Exists(TId id);

        Task<bool> ExistsAsync(TId id);

        void Insert(TEntity entity);

        Task InsertAsync(TEntity entity);

        void Save(TEntity entity);

        Task SaveAsync(TEntity entity);

        void Delete(TId entity);

        Task DeleteAsync(TId entity);
    }
}