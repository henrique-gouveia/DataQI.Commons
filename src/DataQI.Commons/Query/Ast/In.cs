namespace DataQI.Commons.Query.Ast
{
    /// <summary>Represents a test that a property equals one of a set of values.</summary>
    public class In : ICriterion
    {
        /// <summary>Initializes a new instance of the <see cref="In"/> class.</summary>
        /// <param name="propertyName">The name of the entity property to test.</param>
        /// <param name="values">The accepted values.</param>
        public In(string propertyName, object[] values)
        {
            PropertyName = propertyName;
            Values = values;
        }

        /// <summary>Gets the name of the entity property to test.</summary>
        public string PropertyName { get; }
        /// <summary>Gets the accepted values.</summary>
        public object[] Values { get; }

        /// <inheritdoc />
        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
