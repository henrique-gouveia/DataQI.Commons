namespace DataQI.Commons.Query.Ast
{
    /// <summary>Represents the comparison of a text property with a text value.</summary>
    public class TextMatch : ICriterion
    {
        /// <summary>Initializes a new instance of the <see cref="TextMatch"/> class.</summary>
        /// <param name="propertyName">The name of the text property to match.</param>
        /// <param name="kind">How the property is matched against <paramref name="value"/>.</param>
        /// <param name="value">The text to match.</param>
        public TextMatch(string propertyName, TextMatchKind kind, string value)
        {
            PropertyName = propertyName;
            Kind = kind;
            Value = value;
        }

        /// <summary>Gets the name of the text property to match.</summary>
        public string PropertyName { get; }
        /// <summary>Gets how the property is matched against <see cref="Value"/>.</summary>
        public TextMatchKind Kind { get; }
        /// <summary>Gets the text to match.</summary>
        public string Value { get; }

        /// <inheritdoc />
        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
