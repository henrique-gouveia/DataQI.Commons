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
    public static class QueryMethodParser
    {
        private const string PrefixPattern = @"\w+By";
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

        private static readonly IReadOnlyDictionary<PredicateKind, HashSet<string>> Keywords =
            new Dictionary<PredicateKind, HashSet<string>>
            {
                { PredicateKind.Between, new HashSet<string> { "IsBetween", "Between" } },
                { PredicateKind.Containing, new HashSet<string> { "IsContaining", "Containing", "Contains" } },
                { PredicateKind.EndingWith, new HashSet<string> { "IsEndingWith", "EndingWith", "EndsWith" } },
                { PredicateKind.Equal, new HashSet<string> { "IsEqual", "Equal", "IsEquals", "Equals" } },
                { PredicateKind.GreaterThan, new HashSet<string> { "IsGreaterThan", "GreaterThan" } },
                { PredicateKind.GreaterThanEqual, new HashSet<string> { "IsGreaterThanEqual", "GreaterThanEqual" } },
                { PredicateKind.In, new HashSet<string> { "IsIn", "In" } },
                { PredicateKind.LessThan, new HashSet<string> { "IsLessThan", "LessThan" } },
                { PredicateKind.LessThanEqual, new HashSet<string> { "IsLessThanEqual", "LessThanEqual" } },
                { PredicateKind.Like, new HashSet<string> { "IsLike", "Like" } },
                { PredicateKind.Null, new HashSet<string> { "IsNull", "Null" } },
                { PredicateKind.StartingWith, new HashSet<string> { "IsStartingWith", "StartingWith", "StartsWith" } },
            };

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
        {
            if (typeMatch.Success)
                foreach (var entry in Keywords)
                    if (entry.Value.Contains(typeMatch.Value))
                        return entry.Key;

            return PredicateKind.SimpleProperty;
        }

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
                    criterion = new TextMatch(member.PropertyName, TextMatchKind.Containing, (string)Next(values, ref index));
                    break;
                case PredicateKind.EndingWith:
                    criterion = new TextMatch(member.PropertyName, TextMatchKind.EndingWith, (string)Next(values, ref index));
                    break;
                case PredicateKind.Like:
                    criterion = new TextMatch(member.PropertyName, TextMatchKind.Like, (string)Next(values, ref index));
                    break;
                case PredicateKind.StartingWith:
                    criterion = new TextMatch(member.PropertyName, TextMatchKind.StartingWith, (string)Next(values, ref index));
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
