using DataQI.Commons.Query.Support;

namespace DataQI.Commons.Query
{
    public interface IOrderCriterion
    {
        string PropertyName { get; }
        OrderDirection Direction { get; }
    }
}
