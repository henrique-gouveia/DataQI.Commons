namespace DataQI.Commons.Query.Support
{
    public class Disjunction : Junction
    {
        public override WhereOperator GetWhereOperator() => WhereOperator.Or;
    }
}