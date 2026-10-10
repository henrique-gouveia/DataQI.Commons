namespace DataQI.Commons.Query.Support
{
    /// <summary>Provides the default implementation of <see cref="IOrderCriterion"/>.</summary>
    public class OrderCriterion : IOrderCriterion
    {
        /// <summary>Initializes a new instance of the <see cref="OrderCriterion"/> class.</summary>
        /// <param name="propertyName">The name of the entity property to order by.</param>
        /// <param name="direction">The direction of the ordering.</param>
        public OrderCriterion(string propertyName, OrderDirection direction)
        {
            PropertyName = propertyName;
            Direction = direction;
        }

        /// <inheritdoc />
        public string PropertyName { get; }

        /// <inheritdoc />
        public OrderDirection Direction { get; }
    }
}
