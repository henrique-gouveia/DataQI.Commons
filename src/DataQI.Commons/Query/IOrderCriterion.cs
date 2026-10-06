using DataQI.Commons.Query.Support;

namespace DataQI.Commons.Query
{
    public interface IOrderCriterion
    {
        string GetPropertyName();
        OrderDirection GetDirection();
    }
}
