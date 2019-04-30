using Net.Data.Commons.Repository;

namespace Net.Data.Commons.Test.Repository.Sample
{
    public interface IDefaultRepository<TEntity> : ICrudRepository<TEntity, int> where TEntity : class, new()
    {

    }
}
