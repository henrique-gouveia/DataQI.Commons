namespace DataQI.Commons.Query.Support
{
    public class OrderCriterion : IOrderCriterion
    {
        public OrderCriterion(string propertyName, OrderDirection direction)
        {
            PropertyName = propertyName;
            Direction = direction;
        }

        public string PropertyName { get; }

        public OrderDirection Direction { get; }
    }
}
