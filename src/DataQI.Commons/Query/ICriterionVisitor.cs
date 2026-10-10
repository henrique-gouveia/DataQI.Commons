using DataQI.Commons.Query.Ast;
using DataQI.Commons.Query.Support;

namespace DataQI.Commons.Query
{
    /// <summary>
    /// Translates each kind of <see cref="ICriterion"/> node into a provider specific result.
    /// </summary>
    /// <typeparam name="T">The type produced for each node, for example a SQL fragment or an expression tree.</typeparam>
    public interface ICriterionVisitor<out T>
    {
        /// <summary>Translates a <see cref="Comparison"/> node.</summary>
        /// <param name="comparison">The node to translate.</param>
        /// <returns>The translation of <paramref name="comparison"/>.</returns>
        T Visit(Comparison comparison);
        /// <summary>Translates a <see cref="Between"/> node.</summary>
        /// <param name="between">The node to translate.</param>
        /// <returns>The translation of <paramref name="between"/>.</returns>
        T Visit(Between between);
        /// <summary>Translates an <see cref="In"/> node.</summary>
        /// <param name="inCriterion">The node to translate.</param>
        /// <returns>The translation of <paramref name="inCriterion"/>.</returns>
        T Visit(In inCriterion);
        /// <summary>Translates an <see cref="IsNull"/> node.</summary>
        /// <param name="isNull">The node to translate.</param>
        /// <returns>The translation of <paramref name="isNull"/>.</returns>
        T Visit(IsNull isNull);
        /// <summary>Translates a <see cref="TextMatch"/> node.</summary>
        /// <param name="textMatch">The node to translate.</param>
        /// <returns>The translation of <paramref name="textMatch"/>.</returns>
        T Visit(TextMatch textMatch);
        /// <summary>Translates a <see cref="Not"/> node.</summary>
        /// <param name="not">The node to translate.</param>
        /// <returns>The translation of <paramref name="not"/>.</returns>
        T Visit(Not not);
        /// <summary>Translates a <see cref="Junction"/> node.</summary>
        /// <param name="junction">The node to translate.</param>
        /// <returns>The translation of <paramref name="junction"/>.</returns>
        T Visit(Junction junction);
    }
}
