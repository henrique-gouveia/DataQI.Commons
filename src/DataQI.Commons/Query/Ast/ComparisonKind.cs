namespace DataQI.Commons.Query.Ast
{
    /// <summary>
    /// Specifies the operator of a <see cref="Comparison"/> criterion.
    /// </summary>
    public enum ComparisonKind
    {
        /// <summary>Matches when the property is equal to the value.</summary>
        Equal,
        /// <summary>Matches when the property is greater than the value.</summary>
        GreaterThan,
        /// <summary>Matches when the property is greater than or equal to the value.</summary>
        GreaterThanEqual,
        /// <summary>Matches when the property is less than the value.</summary>
        LessThan,
        /// <summary>Matches when the property is less than or equal to the value.</summary>
        LessThanEqual
    }
}
