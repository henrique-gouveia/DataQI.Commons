using System.Collections.Generic;
using System.Collections.ObjectModel;

using DataQI.Commons.Query.Ast.Support;
using DataQI.Commons.Util;

namespace DataQI.Commons.Query.Ast
{
    public class Junction : ICriterion
    {
        private readonly IList<ICriterion> members = new List<ICriterion>();

        public Junction(LogicalKind kind)
        {
            Kind = kind;
        }

        public LogicalKind Kind { get; }

        public Junction Add(ICriterion criterion)
        {
            Assert.NotNull(criterion, "Criterion must not be null");
            members.Add(criterion);
            return this;
        }

        public IReadOnlyCollection<ICriterion> Members => new ReadOnlyCollection<ICriterion>(members);

        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
