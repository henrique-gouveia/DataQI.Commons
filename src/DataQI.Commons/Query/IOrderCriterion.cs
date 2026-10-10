using DataQI.Commons.Query.Support;

namespace DataQI.Commons.Query
{
    /// <summary>
    /// Represents the ordering of a query by one property.
    /// </summary>
    public interface IOrderCriterion
    {
        /// <summary>Gets the name of the entity property to order by.</summary>
        string PropertyName { get; }
        /// <summary>Gets the direction of the ordering.</summary>
        OrderDirection Direction { get; }
    }
}
