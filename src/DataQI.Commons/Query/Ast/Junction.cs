using System.Collections.Generic;
using System.Collections.ObjectModel;

using DataQI.Commons.Util;

namespace DataQI.Commons.Query.Ast
{
    /// <summary>Represents a group of criterions combined with a logical AND or OR.</summary>
    /// <remarks>
    /// A junction must contain at least one member before a provider translates it; translating an empty
    /// junction is not supported.
    /// </remarks>
    public class Junction : ICriterion
    {
        private readonly IList<ICriterion> members = new List<ICriterion>();

        /// <summary>Initializes a new, empty instance of the <see cref="Junction"/> class.</summary>
        /// <param name="kind">How the members are combined.</param>
        public Junction(LogicalKind kind)
        {
            Kind = kind;
        }

        /// <summary>Gets how the members are combined.</summary>
        public LogicalKind Kind { get; }

        /// <summary>Adds a member to the junction.</summary>
        /// <param name="criterion">The criterion to add; must not be <c>null</c>.</param>
        /// <returns>This instance, to allow chaining.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="criterion"/> is <c>null</c>.</exception>
        public Junction Add(ICriterion criterion)
        {
            Assert.NotNull(criterion, "Criterion must not be null");
            members.Add(criterion);
            return this;
        }

        /// <summary>Gets a read-only view of the members, in insertion order.</summary>
        public IReadOnlyCollection<ICriterion> Members => new ReadOnlyCollection<ICriterion>(members);

        /// <inheritdoc />
        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
