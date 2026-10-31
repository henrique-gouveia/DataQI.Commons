namespace DataQI.Commons.Repository
{
    /// <summary>Identifies the entity and identifier types shared by independent repository contracts.</summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The identifier type.</typeparam>
    public interface IRepository<TEntity, in TId> where TEntity : class
    {
    }
}
