using System.Reflection;

namespace Net.Data.Commons.Repository.Support
{
    public class CrudRepositoryProxyFactory<TEntity, TId, TRepositoryProxy>
        where TEntity : class, new()
        where TRepositoryProxy : CrudRepositoryProxy<TEntity, TId>
    {
        public virtual TRepository Create<TRepository>() where TRepository : ICrudRepository<TEntity, TId>
        {
            return DispatchProxy.Create<TRepository, TRepositoryProxy>();
        }
    }
}