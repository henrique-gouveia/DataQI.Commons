namespace Net.Data.Commons.Criteria
{
    public interface ICriteria<TEntity> where TEntity : class, new()
    {
         ICriteria<TEntity> Add(ICriterion criterion);

         string ToSqlString();
    }
}