using DataQI.Commons.Query.Ast.Support;

namespace DataQI.Commons.Query.Ast
{
    public class TextMatch : ICriterion
    {
        public TextMatch(string propertyName, TextMatchKind kind, string value)
        {
            PropertyName = propertyName;
            Kind = kind;
            Value = value;
        }

        public string PropertyName { get; }
        public TextMatchKind Kind { get; }
        public string Value { get; }

        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
