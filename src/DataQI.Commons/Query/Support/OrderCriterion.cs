namespace DataQI.Commons.Query.Support
{
    public class OrderCriterion : IOrderCriterion
    {
        private readonly string propertyName;
        private readonly OrderDirection direction;

        public OrderCriterion(string propertyName, OrderDirection direction)
        {
            this.propertyName = propertyName;
            this.direction = direction;
        }

        public string GetPropertyName() => propertyName;

        public OrderDirection GetDirection() => direction;
    }
}
