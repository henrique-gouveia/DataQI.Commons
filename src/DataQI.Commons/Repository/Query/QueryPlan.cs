using System;
using System.Collections.Generic;

using DataQI.Commons.Query;
using DataQI.Commons.Util;


namespace DataQI.Commons.Repository.Query
{
    /// <summary>Represents a parsed query method name, ready to be applied to argument values.</summary>
    /// <remarks>Instances are created by <see cref="QueryMethodParser.Parse(System.Reflection.MethodInfo)"/> and can be reused with different argument values.</remarks>
    public sealed class QueryPlan
    {
        private readonly Func<object[], ICriterion> predicateFactory;

        internal QueryPlan(Func<object[], ICriterion> predicateFactory, IReadOnlyList<IOrderCriterion> orders)
        {
            this.predicateFactory = predicateFactory;
            Orders = orders;
        }

        /// <summary>Gets the orderings declared after <c>OrderBy</c>, in declaration order.</summary>
        public IReadOnlyList<IOrderCriterion> Orders { get; }

        /// <summary>Adds the predicate built from <paramref name="values"/> and the orderings to <paramref name="criteria"/>.</summary>
        /// <param name="criteria">The criteria to fill; must not be <c>null</c>.</param>
        /// <param name="values">The method arguments, in the order the keywords consume them.</param>
        /// <exception cref="System.ArgumentException"><paramref name="criteria"/> is <c>null</c>; see <see cref="BuildPredicate(object[])"/> for the errors caused by <paramref name="values"/>.</exception>
        public void ApplyTo(ICriteria criteria, object[] values)
        {
            Assert.NotNull(criteria, "Criteria must be not null");

            criteria.Add(BuildPredicate(values));
            foreach (var order in Orders)
                criteria.AddOrder(order);
        }

        /// <summary>Builds the predicate for the given argument values.</summary>
        /// <param name="values">The method arguments, in the order the keywords consume them; must not be <c>null</c>.</param>
        /// <returns>An <see cref="DataQI.Commons.Query.Ast.Junction"/> of kind <see cref="DataQI.Commons.Query.Ast.LogicalKind.Or"/> whose members are <see cref="DataQI.Commons.Query.Ast.Junction"/>s of kind <see cref="DataQI.Commons.Query.Ast.LogicalKind.And"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="values"/> is <c>null</c>, holds fewer values than the name requires, or holds a value that is not a <see cref="string"/> for a text keyword (<c>Containing</c>, <c>StartingWith</c>, <c>EndingWith</c>, <c>Like</c>); <c>null</c> is accepted for text keywords.</exception>
        /// <exception cref="System.InvalidCastException">The value for an <c>In</c> keyword is not an <c>object[]</c>.</exception>
        public ICriterion BuildPredicate(object[] values)
        {
            Assert.NotNull(values, "Query Values must not be null");
            return predicateFactory(values);
        }
    }
}
