namespace DataQI.Commons.Query.Ast
{
    /// <summary>
    /// Specifies how a <see cref="Junction"/> combines its members.
    /// </summary>
    public enum LogicalKind
    {
        /// <summary>Requires every member to match.</summary>
        And,
        /// <summary>Requires at least one member to match.</summary>
        Or
    }
}
