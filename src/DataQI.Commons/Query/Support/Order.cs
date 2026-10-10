namespace DataQI.Commons.Query.Support
{
    /// <summary>Creates <see cref="IOrderCriterion"/> instances.</summary>
    public static class Order
    {
        /// <summary>Creates an ascending ordering.</summary>
        /// <param name="propertyName">The name of the entity property to order by.</param>
        /// <returns>An ordering with <see cref="OrderDirection.Asc"/> direction.</returns>
        public static IOrderCriterion Asc(string propertyName) => new OrderCriterion(propertyName, OrderDirection.Asc);

        /// <summary>Creates a descending ordering.</summary>
        /// <param name="propertyName">The name of the entity property to order by.</param>
        /// <returns>An ordering with <see cref="OrderDirection.Desc"/> direction.</returns>
        public static IOrderCriterion Desc(string propertyName) => new OrderCriterion(propertyName, OrderDirection.Desc);
    }
}
