namespace DataQI.Commons.Query.Ast
{
    /// <summary>Represents the comparison of a property with a single value.</summary>
    public class Comparison : ICriterion
    {
        /// <summary>Initializes a new instance of the <see cref="Comparison"/> class.</summary>
        /// <param name="propertyName">The name of the entity property to compare.</param>
        /// <param name="kind">The comparison operator.</param>
        /// <param name="value">The value to compare the property with.</param>
        public Comparison(string propertyName, ComparisonKind kind, object value)
        {
            PropertyName = propertyName;
            Kind = kind;
            Value = value;
        }

        /// <summary>Gets the name of the entity property to compare.</summary>
        public string PropertyName { get; }
        /// <summary>Gets the comparison operator.</summary>
        public ComparisonKind Kind { get; }
        /// <summary>Gets the comparison value, which the provider converts to the property type when needed.</summary>
        public object Value { get; }

        /// <inheritdoc />
        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
