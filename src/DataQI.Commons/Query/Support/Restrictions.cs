namespace DataQI.Commons.Query.Support
{
    public static class Restrictions
    {
        public static ICriterion Between(string propertyName, object starts, object ends)
            => new BetweenExpression(propertyName, starts, ends);

        public static ICriterion Containing(string propertyName, object value)
            => new SimpleExpression(propertyName, WhereOperator.Containing, value);

        public static ICriterion EndingWith(string propertyName, string value)
            => new SimpleExpression(propertyName, WhereOperator.EndingWith, value);

        public static ICriterion Equal(string propertyName, object value)
            => new SimpleExpression(propertyName, WhereOperator.Equal, value);

        public static ICriterion GreaterThan(string propertyName, object value)
            => new SimpleExpression(propertyName, WhereOperator.GreaterThan, value);

        public static ICriterion GreaterThanEqual(string propertyName, object value)
            => new SimpleExpression(propertyName, WhereOperator.GreaterThanEqual, value);

        public static ICriterion In(string propertyName, object[] values)
            => new InExpression(propertyName, values);

        public static ICriterion LessThan(string propertyName, object value)
            => new SimpleExpression(propertyName, WhereOperator.LessThan, value);

        public static ICriterion LessThanEqual(string propertyName, object value)
            => new SimpleExpression(propertyName, WhereOperator.LessThanEqual, value);

        public static ICriterion Like(string propertyName, string value)
            => new SimpleExpression(propertyName, WhereOperator.Like, value);

        public static ICriterion Not(ICriterion criterion)
            => new NotExpression(criterion);

        public static ICriterion Null(string propertyName)
            => new NullExpression(propertyName);

        public static ICriterion StartingWith(string propertyName, object value)
            => new SimpleExpression(propertyName, WhereOperator.StartingWith, value);

        public static IJunction Conjunction()
            => new Conjunction();

        public static IJunction Disjunction()
            => new Disjunction();
    }
}