using DataQI.Commons.Repository;

namespace DataQI.Commons.Test.Repository.Sample
{
    public interface IDefaultRepository<TEntity> : ICrudRepository<TEntity, int> 
        where TEntity : class, new()
    {

    }
}
