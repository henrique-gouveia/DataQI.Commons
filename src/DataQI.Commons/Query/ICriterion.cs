using DataQI.Commons.Query.Support;

namespace DataQI.Commons.Query
{
    public interface ICriterion
    {
        string GetPropertyName();
        WhereOperator GetWhereOperator();
    }
}