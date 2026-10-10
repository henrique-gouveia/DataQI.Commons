namespace DataQI.Commons.Query.Ast
{
    /// <summary>Represents a test that a property is <c>null</c>.</summary>
    public class IsNull : ICriterion
    {
        /// <summary>Initializes a new instance of the <see cref="IsNull"/> class.</summary>
        /// <param name="propertyName">The name of the entity property to test.</param>
        public IsNull(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>Gets the name of the entity property to test.</summary>
        public string PropertyName { get; }

        /// <inheritdoc />
        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
