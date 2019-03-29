namespace Net.Data.Commons.Criterions
{
    public interface ICriteria
    {
         ICriteria Add(ICriterion criterion);

         string ToSqlString();
    }
}