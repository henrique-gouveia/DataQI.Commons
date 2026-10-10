using DataQI.Commons.Query.Ast;

namespace DataQI.Commons.Query.Support
{
    /// <summary>Creates criteria AST nodes through short, readable factory methods.</summary>
    /// <example>
    /// <code>
    /// var people = repository.Find(criteria => criteria
    ///     .Add(Restrictions.Equal("LastName", "Adams"))
    ///     .Add(Restrictions.GreaterThan("Age", 30))
    ///     .AddOrder(Order.Asc("FirstName")));
    /// </code>
    /// </example>
    public static class Restrictions
    {
        /// <summary>Creates a criterion that matches when the property lies within an inclusive range.</summary>
        /// <param name="propertyName">The name of the entity property to test.</param>
        /// <param name="starts">The inclusive lower bound.</param>
        /// <param name="ends">The inclusive upper bound.</param>
        /// <returns>A <see cref="Ast.Between"/> criterion.</returns>
        public static ICriterion Between(string propertyName, object starts, object ends)
            => new Between(propertyName, starts, ends);

        /// <summary>Creates a criterion that matches when the text property contains the value.</summary>
        /// <param name="propertyName">The name of the text property to match.</param>
        /// <param name="value">The text to look for.</param>
        /// <returns>A <see cref="TextMatch"/> with <see cref="TextMatchKind.Containing"/> kind.</returns>
        public static ICriterion Containing(string propertyName, string value)
            => new TextMatch(propertyName, TextMatchKind.Containing, value);

        /// <summary>Creates a criterion that matches when the text property ends with the value.</summary>
        /// <param name="propertyName">The name of the text property to match.</param>
        /// <param name="value">The text to look for.</param>
        /// <returns>A <see cref="TextMatch"/> with <see cref="TextMatchKind.EndingWith"/> kind.</returns>
        public static ICriterion EndingWith(string propertyName, string value)
            => new TextMatch(propertyName, TextMatchKind.EndingWith, value);

        /// <summary>Creates a criterion that matches when the property is equal to the value.</summary>
        /// <param name="propertyName">The name of the entity property to compare.</param>
        /// <param name="value">The value to compare the property with.</param>
        /// <returns>A <see cref="Comparison"/> with <see cref="ComparisonKind.Equal"/> kind.</returns>
        public static ICriterion Equal(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.Equal, value);

        /// <summary>Creates a criterion that matches when the property is greater than the value.</summary>
        /// <param name="propertyName">The name of the entity property to compare.</param>
        /// <param name="value">The value to compare the property with.</param>
        /// <returns>A <see cref="Comparison"/> with <see cref="ComparisonKind.GreaterThan"/> kind.</returns>
        public static ICriterion GreaterThan(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.GreaterThan, value);

        /// <summary>Creates a criterion that matches when the property is greater than or equal to the value.</summary>
        /// <param name="propertyName">The name of the entity property to compare.</param>
        /// <param name="value">The value to compare the property with.</param>
        /// <returns>A <see cref="Comparison"/> with <see cref="ComparisonKind.GreaterThanEqual"/> kind.</returns>
        public static ICriterion GreaterThanEqual(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.GreaterThanEqual, value);

        /// <summary>Creates a criterion that matches when the property equals one of the values.</summary>
        /// <param name="propertyName">The name of the entity property to test.</param>
        /// <param name="values">The accepted values.</param>
        /// <returns>An <see cref="Ast.In"/> criterion.</returns>
        public static ICriterion In(string propertyName, object[] values)
            => new In(propertyName, values);

        /// <summary>Creates a criterion that matches when the property is less than the value.</summary>
        /// <param name="propertyName">The name of the entity property to compare.</param>
        /// <param name="value">The value to compare the property with.</param>
        /// <returns>A <see cref="Comparison"/> with <see cref="ComparisonKind.LessThan"/> kind.</returns>
        public static ICriterion LessThan(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.LessThan, value);

        /// <summary>Creates a criterion that matches when the property is less than or equal to the value.</summary>
        /// <param name="propertyName">The name of the entity property to compare.</param>
        /// <param name="value">The value to compare the property with.</param>
        /// <returns>A <see cref="Comparison"/> with <see cref="ComparisonKind.LessThanEqual"/> kind.</returns>
        public static ICriterion LessThanEqual(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.LessThanEqual, value);

        /// <summary>Creates a criterion that matches when the text property matches the pattern.</summary>
        /// <param name="propertyName">The name of the text property to match.</param>
        /// <param name="value">The pattern to match; the wildcard syntax is defined by the provider.</param>
        /// <returns>A <see cref="TextMatch"/> with <see cref="TextMatchKind.Like"/> kind.</returns>
        public static ICriterion Like(string propertyName, string value)
            => new TextMatch(propertyName, TextMatchKind.Like, value);

        /// <summary>Creates a criterion that negates another criterion.</summary>
        /// <param name="criterion">The criterion to negate.</param>
        /// <returns>A <see cref="Ast.Not"/> criterion.</returns>
        public static ICriterion Not(ICriterion criterion)
            => new Not(criterion);

        /// <summary>Creates a criterion that matches when the property is <c>null</c>.</summary>
        /// <param name="propertyName">The name of the entity property to test.</param>
        /// <returns>An <see cref="IsNull"/> criterion.</returns>
        public static ICriterion Null(string propertyName)
            => new IsNull(propertyName);

        /// <summary>Creates a criterion that matches when the text property starts with the value.</summary>
        /// <param name="propertyName">The name of the text property to match.</param>
        /// <param name="value">The text to look for.</param>
        /// <returns>A <see cref="TextMatch"/> with <see cref="TextMatchKind.StartingWith"/> kind.</returns>
        public static ICriterion StartingWith(string propertyName, string value)
            => new TextMatch(propertyName, TextMatchKind.StartingWith, value);

        /// <summary>Creates an empty junction whose members must all match.</summary>
        /// <returns>A <see cref="Junction"/> with <see cref="LogicalKind.And"/> kind; add members with <see cref="Junction.Add(ICriterion)"/>.</returns>
        public static Junction Conjunction()
            => new Junction(LogicalKind.And);

        /// <summary>Creates an empty junction of which at least one member must match.</summary>
        /// <returns>A <see cref="Junction"/> with <see cref="LogicalKind.Or"/> kind; add members with <see cref="Junction.Add(ICriterion)"/>.</returns>
        public static Junction Disjunction()
            => new Junction(LogicalKind.Or);
    }
}
