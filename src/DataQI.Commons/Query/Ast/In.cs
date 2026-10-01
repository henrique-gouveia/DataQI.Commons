namespace DataQI.Commons.Query.Ast
{
    public class In : ICriterion
    {
        public In(string propertyName, object[] values)
        {
            PropertyName = propertyName;
            Values = values;
        }

        public string PropertyName { get; }
        public object[] Values { get; }

        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
