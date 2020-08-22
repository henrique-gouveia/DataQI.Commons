using System;

namespace DataQI.Commons.Query.Support
{
    public class BetweenExpression : Criterion
    {
        public BetweenExpression(string propertyName, object starts, object ends)
            : base(propertyName, WhereOperator.Between)
        {
            Starts = starts;
            Ends = ends;
        }

        public object Starts { get; }
        public object Ends { get; }
    }
}