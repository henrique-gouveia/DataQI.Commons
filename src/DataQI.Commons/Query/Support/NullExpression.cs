namespace DataQI.Commons.Query.Support
{
    public class NullExpression : Criterion
    {
        public NullExpression(string propertyName)
            : base(propertyName, WhereOperator.Null)
        { 
            
        }
    }
}