namespace DataQI.Commons.Query.Support
{
    public static class Order
    {
        public static IOrderCriterion Asc(string propertyName) => new OrderCriterion(propertyName, OrderDirection.Asc);

        public static IOrderCriterion Desc(string propertyName) => new OrderCriterion(propertyName, OrderDirection.Desc);
    }
}
