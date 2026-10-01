using DataQI.Commons.Query.Ast;

namespace DataQI.Commons.Query.Support
{
    public static class Restrictions
    {
        public static ICriterion Between(string propertyName, object starts, object ends)
            => new Between(propertyName, starts, ends);

        public static ICriterion Containing(string propertyName, string value)
            => new TextMatch(propertyName, TextMatchKind.Containing, value);

        public static ICriterion EndingWith(string propertyName, string value)
            => new TextMatch(propertyName, TextMatchKind.EndingWith, value);

        public static ICriterion Equal(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.Equal, value);

        public static ICriterion GreaterThan(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.GreaterThan, value);

        public static ICriterion GreaterThanEqual(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.GreaterThanEqual, value);

        public static ICriterion In(string propertyName, object[] values)
            => new In(propertyName, values);

        public static ICriterion LessThan(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.LessThan, value);

        public static ICriterion LessThanEqual(string propertyName, object value)
            => new Comparison(propertyName, ComparisonKind.LessThanEqual, value);

        public static ICriterion Like(string propertyName, string value)
            => new TextMatch(propertyName, TextMatchKind.Like, value);

        public static ICriterion Not(ICriterion criterion)
            => new Not(criterion);

        public static ICriterion Null(string propertyName)
            => new IsNull(propertyName);

        public static ICriterion StartingWith(string propertyName, string value)
            => new TextMatch(propertyName, TextMatchKind.StartingWith, value);

        public static Junction Conjunction()
            => new Junction(LogicalKind.And);

        public static Junction Disjunction()
            => new Junction(LogicalKind.Or);
    }
}
