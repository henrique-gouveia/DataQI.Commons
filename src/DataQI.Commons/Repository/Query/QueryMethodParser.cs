using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Ast;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Util;


namespace DataQI.Commons.Repository.Query
{
    /// <summary>Parses the name of a repository query method into a <see cref="QueryPlan"/>.</summary>
    /// <remarks>
    /// <para>
    /// A name has the form <c>&lt;Prefix&gt;By&lt;Predicate&gt;[OrderBy&lt;Orders&gt;][Async]</c>. The prefix is any word
    /// followed by <c>By</c> and an uppercase letter, conventionally <c>Find</c>. A trailing <c>Async</c> is ignored.
    /// </para>
    /// <para>
    /// The predicate is split on <c>Or</c>, then on <c>And</c> (each only when followed by an uppercase letter), so
    /// <c>FindByFirstNameAndLastNameOrAge</c> means <c>(FirstName AND LastName) OR Age</c>. Each part is a property
    /// name optionally followed by a keyword; without a keyword the part is an equality test. Putting <c>Not</c> in a
    /// part negates it (<c>NameNotLike</c>), and the keywords also accept an <c>Is</c> prefix (<c>AgeIsGreaterThan</c>).
    /// </para>
    /// <list type="table">
    /// <listheader><term>Keyword</term><description>Criterion and number of arguments consumed</description></listheader>
    /// <item><term><c>Between</c></term><description><see cref="DataQI.Commons.Query.Ast.Between"/>, two arguments (inclusive bounds).</description></item>
    /// <item><term><c>Containing</c>, <c>Contains</c></term><description><see cref="DataQI.Commons.Query.Ast.TextMatch"/> with <see cref="DataQI.Commons.Query.Ast.TextMatchKind.Containing"/>, one <see cref="string"/>.</description></item>
    /// <item><term><c>StartingWith</c>, <c>StartsWith</c></term><description><see cref="DataQI.Commons.Query.Ast.TextMatch"/> with <see cref="DataQI.Commons.Query.Ast.TextMatchKind.StartingWith"/>, one <see cref="string"/>.</description></item>
    /// <item><term><c>EndingWith</c>, <c>EndsWith</c></term><description><see cref="DataQI.Commons.Query.Ast.TextMatch"/> with <see cref="DataQI.Commons.Query.Ast.TextMatchKind.EndingWith"/>, one <see cref="string"/>.</description></item>
    /// <item><term><c>Like</c></term><description><see cref="DataQI.Commons.Query.Ast.TextMatch"/> with <see cref="DataQI.Commons.Query.Ast.TextMatchKind.Like"/>, one <see cref="string"/>.</description></item>
    /// <item><term><c>Equal</c>, <c>Equals</c></term><description><see cref="DataQI.Commons.Query.Ast.Comparison"/> with <see cref="DataQI.Commons.Query.Ast.ComparisonKind.Equal"/>, one argument.</description></item>
    /// <item><term><c>GreaterThan</c>, <c>GreaterThanEqual</c>, <c>LessThan</c>, <c>LessThanEqual</c></term><description><see cref="DataQI.Commons.Query.Ast.Comparison"/> with the matching <see cref="DataQI.Commons.Query.Ast.ComparisonKind"/>, one argument.</description></item>
    /// <item><term><c>In</c></term><description><see cref="DataQI.Commons.Query.Ast.In"/>, one <c>object[]</c> argument.</description></item>
    /// <item><term><c>Null</c></term><description><see cref="DataQI.Commons.Query.Ast.IsNull"/>, no argument.</description></item>
    /// </list>
    /// <para>
    /// Arguments are consumed from left to right in the order the keywords appear. After <c>OrderBy</c>, each
    /// property may be followed by <c>Asc</c> or <c>Desc</c> (default <c>Asc</c>), for example
    /// <c>FindByLastNameOrderByFirstNameDescAgeAsc</c>.
    /// </para>
    /// </remarks>
    public static class QueryMethodParser
    {
        private const string PrefixPattern = @"^\w+?By(?=[A-Z])";
        private const string EmptyPredicatePattern = @"^\w+By$";
        private const string AsyncSuffixPattern = "Async$";
        private const string OrSeparatorPattern = "Or(?=[A-Z])";
        private const string AndSeparatorPattern = "And(?=[A-Z])";

        private static readonly string UppercaseOrEndPattern = "(?=[A-Z]|$)";
        private static readonly Regex OrderByMatcher = new Regex($"OrderBy{UppercaseOrEndPattern}");
        private static readonly Regex DirectionMatcher = new Regex($"(Asc|Desc){UppercaseOrEndPattern}");

        private static readonly string IsNotSmallLettersPattern = "(?!([a-z]))";
        private static readonly string LessOrGreaterGroupPattern = "(Less|Greater)+Than(Equal)?";
        private static readonly string LikeGroupPattern = "Contain(s|ing)|Like|((End|Start)+(s|ing)With)";
        private static readonly string NotPattern = "Not";
        private static readonly string NotGroupPattern = $"({NotPattern})?(Null|Equals?|Between|In|{LikeGroupPattern})";
        private static readonly string TypePattern = $"((Is)?({NotGroupPattern}|{LessOrGreaterGroupPattern})){IsNotSmallLettersPattern}";

        private static readonly Regex NotMatcher = new Regex($"{NotPattern}{IsNotSmallLettersPattern}");
        private static readonly Regex TypeMatcher = new Regex(TypePattern);

        private static readonly IReadOnlyDictionary<string, PredicateKind> Keywords =
            new Dictionary<string, PredicateKind>
            {
                { "IsBetween", PredicateKind.Between },
                { "Between", PredicateKind.Between },
                { "IsContaining", PredicateKind.Containing },
                { "Containing", PredicateKind.Containing },
                { "Contains", PredicateKind.Containing },
                { "IsEndingWith", PredicateKind.EndingWith },
                { "EndingWith", PredicateKind.EndingWith },
                { "EndsWith", PredicateKind.EndingWith },
                { "IsEqual", PredicateKind.Equal },
                { "Equal", PredicateKind.Equal },
                { "IsEquals", PredicateKind.Equal },
                { "Equals", PredicateKind.Equal },
                { "IsGreaterThan", PredicateKind.GreaterThan },
                { "GreaterThan", PredicateKind.GreaterThan },
                { "IsGreaterThanEqual", PredicateKind.GreaterThanEqual },
                { "GreaterThanEqual", PredicateKind.GreaterThanEqual },
                { "IsIn", PredicateKind.In },
                { "In", PredicateKind.In },
                { "IsLessThan", PredicateKind.LessThan },
                { "LessThan", PredicateKind.LessThan },
                { "IsLessThanEqual", PredicateKind.LessThanEqual },
                { "LessThanEqual", PredicateKind.LessThanEqual },
                { "IsLike", PredicateKind.Like },
                { "Like", PredicateKind.Like },
                { "IsNull", PredicateKind.Null },
                { "Null", PredicateKind.Null },
                { "IsStartingWith", PredicateKind.StartingWith },
                { "StartingWith", PredicateKind.StartingWith },
                { "StartsWith", PredicateKind.StartingWith },
            };

        /// <summary>Parses the name of a query method.</summary>
        /// <param name="method">The query method whose name is parsed; must not be <c>null</c>.</param>
        /// <returns>A reusable plan that builds the criterion and orders for any argument list.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="method"/> is <c>null</c>, or its name has no predicate after the <c>By</c> prefix.</exception>
        public static QueryPlan Parse(MethodInfo method)
        {
            Assert.NotNull(method, "Query Method must not be null");
            return Parse(method.Name);
        }

        internal static QueryPlan Parse(string methodName)
        {
            Assert.NotNullOrEmpty(methodName, "Source must not be null or empty");

            var source = Regex.Replace(methodName, AsyncSuffixPattern, "");

            var orderSource = "";
            var orderByMatch = OrderByMatcher.Match(source);
            if (orderByMatch.Success)
            {
                orderSource = source.Substring(orderByMatch.Index + orderByMatch.Length);
                source = source.Substring(0, orderByMatch.Index);
            }

            var orders = ParseOrders(orderSource);

            var prefix = Regex.Match(source, PrefixPattern);
            if (!prefix.Success && Regex.IsMatch(source, EmptyPredicatePattern))
                throw new ArgumentException("Source must not be null or empty");

            var groups = ParsePredicate(source.Substring(prefix.Length));

            return new QueryPlan(values => BuildPredicate(groups, values), orders);
        }

        private static IReadOnlyList<IOrderCriterion> ParseOrders(string orderSource)
        {
            var orders = new List<IOrderCriterion>();

            var currentIndex = 0;
            foreach (Match directionMatch in DirectionMatcher.Matches(orderSource))
            {
                var propertyName = orderSource.Substring(currentIndex, directionMatch.Index - currentIndex);
                orders.Add(directionMatch.Value == "Asc" ? Order.Asc(propertyName) : Order.Desc(propertyName));
                currentIndex = directionMatch.Index + directionMatch.Length;
            }

            if (currentIndex < orderSource.Length)
                orders.Add(Order.Asc(orderSource.Substring(currentIndex)));

            return orders;
        }

        private static List<List<PredicateMember>> ParsePredicate(string predicate)
        {
            var groups = new List<List<PredicateMember>>();
            foreach (var orSource in Regex.Split(predicate, OrSeparatorPattern))
            {
                var members = new List<PredicateMember>();
                foreach (var andSource in Regex.Split(orSource, AndSeparatorPattern))
                    members.Add(ParseMember(andSource));
                groups.Add(members);
            }

            return groups;
        }

        private static PredicateMember ParseMember(string source)
        {
            Assert.NotNullOrEmpty(source, "Source must not be null or empty");

            var hasNot = NotMatcher.IsMatch(source);
            if (hasNot)
                source = NotMatcher.Replace(source, "");

            var typeMatch = TypeMatcher.Match(source);
            var kind = KindOf(typeMatch);
            var propertyName = kind == PredicateKind.SimpleProperty
                ? source
                : source.Substring(0, typeMatch.Index);

            return new PredicateMember(propertyName, kind, hasNot);
        }

        private static PredicateKind KindOf(Match typeMatch)
            => typeMatch.Success && Keywords.TryGetValue(typeMatch.Value, out var kind)
                ? kind
                : PredicateKind.SimpleProperty;

        private static ICriterion BuildPredicate(List<List<PredicateMember>> groups, object[] values)
        {
            var index = 0;
            var or = new Junction(LogicalKind.Or);

            foreach (var group in groups)
            {
                var and = new Junction(LogicalKind.And);
                foreach (var member in group)
                    and.Add(BuildCriterion(member, values, ref index));
                or.Add(and);
            }

            return or;
        }

        private static ICriterion BuildCriterion(PredicateMember member, object[] values, ref int index)
        {
            ICriterion criterion;

            switch (member.Kind)
            {
                case PredicateKind.Between:
                    criterion = new Between(member.PropertyName, Next(values, ref index), Next(values, ref index));
                    break;
                case PredicateKind.In:
                    criterion = new In(member.PropertyName, (object[])Next(values, ref index));
                    break;
                case PredicateKind.Null:
                    criterion = new IsNull(member.PropertyName);
                    break;
                case PredicateKind.Containing:
                    criterion = new TextMatch(member.PropertyName, TextMatchKind.Containing, NextTextValue(member.PropertyName, values, ref index));
                    break;
                case PredicateKind.EndingWith:
                    criterion = new TextMatch(member.PropertyName, TextMatchKind.EndingWith, NextTextValue(member.PropertyName, values, ref index));
                    break;
                case PredicateKind.Like:
                    criterion = new TextMatch(member.PropertyName, TextMatchKind.Like, NextTextValue(member.PropertyName, values, ref index));
                    break;
                case PredicateKind.StartingWith:
                    criterion = new TextMatch(member.PropertyName, TextMatchKind.StartingWith, NextTextValue(member.PropertyName, values, ref index));
                    break;
                case PredicateKind.GreaterThan:
                    criterion = new Comparison(member.PropertyName, ComparisonKind.GreaterThan, Next(values, ref index));
                    break;
                case PredicateKind.GreaterThanEqual:
                    criterion = new Comparison(member.PropertyName, ComparisonKind.GreaterThanEqual, Next(values, ref index));
                    break;
                case PredicateKind.LessThan:
                    criterion = new Comparison(member.PropertyName, ComparisonKind.LessThan, Next(values, ref index));
                    break;
                case PredicateKind.LessThanEqual:
                    criterion = new Comparison(member.PropertyName, ComparisonKind.LessThanEqual, Next(values, ref index));
                    break;
                case PredicateKind.Equal:
                case PredicateKind.SimpleProperty:
                default:
                    criterion = new Comparison(member.PropertyName, ComparisonKind.Equal, Next(values, ref index));
                    break;
            }

            return member.HasNot ? new Not(criterion) : criterion;
        }

        private static string NextTextValue(string propertyName, object[] values, ref int index)
        {
            var value = Next(values, ref index);
            if (value != null && !(value is string))
                throw new ArgumentException($"Value for text criterion on property '{propertyName}' must be a string.", nameof(values));

            return (string)value;
        }

        private static object Next(object[] values, ref int index)
        {
            if (index >= values.Length)
                throw new ArgumentException("The query method needs more values than were supplied");

            return values[index++];
        }

        private enum PredicateKind
        {
            Between, Containing, EndingWith, Equal, GreaterThan, GreaterThanEqual,
            In, LessThan, LessThanEqual, Like, Null, StartingWith, SimpleProperty
        }

        private sealed class PredicateMember
        {
            public PredicateMember(string propertyName, PredicateKind kind, bool hasNot)
            {
                PropertyName = propertyName;
                Kind = kind;
                HasNot = hasNot;
            }

            public string PropertyName { get; }
            public PredicateKind Kind { get; }
            public bool HasNot { get; }
        }
    }
}
