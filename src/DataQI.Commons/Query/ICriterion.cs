namespace DataQI.Commons.Query
{
    /// <summary>
    /// Represents a node of the criteria abstract syntax tree (AST).
    /// </summary>
    /// <remarks>
    /// A node only carries data (property names, operators and values); it knows nothing about SQL or LINQ.
    /// A provider translates a tree by passing an <see cref="ICriterionVisitor{T}"/> to
    /// <see cref="Accept{T}(ICriterionVisitor{T})"/>.
    /// </remarks>
    public interface ICriterion
    {
        /// <summary>Dispatches this node to the matching <c>Visit</c> overload of <paramref name="visitor"/>.</summary>
        /// <typeparam name="T">The type produced by the visitor.</typeparam>
        /// <param name="visitor">The visitor that translates this node.</param>
        /// <returns>The value returned by the visitor for this node.</returns>
        T Accept<T>(ICriterionVisitor<T> visitor);
    }
}
