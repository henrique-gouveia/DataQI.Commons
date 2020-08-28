namespace DataQI.Commons.Query.Support
{
    public class NotExpression : Criterion
    {
        public NotExpression(ICriterion criterion) 
            : base(criterion.GetPropertyName(), WhereOperator.Not)
        {
            Criterion = criterion;
        }

        public ICriterion Criterion { get; }
    }
}