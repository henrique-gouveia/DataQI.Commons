namespace DataQI.Commons.Query.Ast
{
    /// <summary>Represents the negation of another criterion.</summary>
    public class Not : ICriterion
    {
        /// <summary>Initializes a new instance of the <see cref="Not"/> class.</summary>
        /// <param name="inner">The criterion to negate.</param>
        public Not(ICriterion inner)
        {
            Inner = inner;
        }

        /// <summary>Gets the negated criterion.</summary>
        public ICriterion Inner { get; }

        /// <inheritdoc />
        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
