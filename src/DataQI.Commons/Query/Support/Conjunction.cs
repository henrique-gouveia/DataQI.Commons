namespace DataQI.Commons.Query.Support
{
    public class Conjunction : Junction
    {
        public override WhereOperator GetWhereOperator() => WhereOperator.And;
    }
}