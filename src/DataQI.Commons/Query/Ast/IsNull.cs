namespace DataQI.Commons.Query.Ast
{
    public class IsNull : ICriterion
    {
        public IsNull(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
