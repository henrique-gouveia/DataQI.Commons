namespace DataQI.Commons.Query.Ast
{
    /// <summary>Represents a test that a property lies within a range whose bounds are both inclusive.</summary>
    public class Between : ICriterion
    {
        /// <summary>Initializes a new instance of the <see cref="Between"/> class.</summary>
        /// <param name="propertyName">The name of the entity property to test.</param>
        /// <param name="starts">The inclusive lower bound.</param>
        /// <param name="ends">The inclusive upper bound.</param>
        public Between(string propertyName, object starts, object ends)
        {
            PropertyName = propertyName;
            Starts = starts;
            Ends = ends;
        }

        /// <summary>Gets the name of the entity property to test.</summary>
        public string PropertyName { get; }
        /// <summary>Gets the inclusive lower bound.</summary>
        public object Starts { get; }
        /// <summary>Gets the inclusive upper bound.</summary>
        public object Ends { get; }

        /// <inheritdoc />
        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
