namespace DataQI.Commons.Query.Support
{
    public class SimpleExpression : Criterion
    {
        public SimpleExpression(string propertyName, WhereOperator whereOperator, object value)
            : base(propertyName, whereOperator)
        {
            Value = value;    
        }

        public object Value { get; }
    }
}