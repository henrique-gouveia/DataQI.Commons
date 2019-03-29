namespace Net.Data.Commons.Criterions
{
    public interface ICriteria
    {

        ICriteria Add(ICriterion criterion);

        ICriteria WithParameters(dynamic parameters);

        string ToSqlString();
        
        object Parameters { get; }
    }
}