namespace DataQI.Commons.Query.Ast
{
    /// <summary>
    /// Specifies how a <see cref="TextMatch"/> criterion compares a text property with its value.
    /// </summary>
    public enum TextMatchKind
    {
        /// <summary>Matches when the property matches the pattern supplied by the caller; the wildcard syntax is defined by the provider.</summary>
        Like,
        /// <summary>Matches when the property contains the value.</summary>
        Containing,
        /// <summary>Matches when the property starts with the value.</summary>
        StartingWith,
        /// <summary>Matches when the property ends with the value.</summary>
        EndingWith
    }
}
