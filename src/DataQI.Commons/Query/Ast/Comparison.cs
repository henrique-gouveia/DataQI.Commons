using DataQI.Commons.Query.Ast.Support;

namespace DataQI.Commons.Query.Ast
{
    public class Comparison : ICriterion
    {
        public Comparison(string propertyName, ComparisonKind kind, object value)
        {
            PropertyName = propertyName;
            Kind = kind;
            Value = value;
        }

        public string PropertyName { get; }
        public ComparisonKind Kind { get; }
        public object Value { get; }

        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
