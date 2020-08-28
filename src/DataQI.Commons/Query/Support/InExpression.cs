namespace DataQI.Commons.Query.Support
{
    public class InExpression : Criterion
    {
        public InExpression(string propertyName, object[] values)
            : base(propertyName, WhereOperator.In)
        {
            Values = values;
        }

        public object[] Values { get; }
    }
}