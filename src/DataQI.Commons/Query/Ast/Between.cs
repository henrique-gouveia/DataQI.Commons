namespace DataQI.Commons.Query.Ast
{
    public class Between : ICriterion
    {
        public Between(string propertyName, object starts, object ends)
        {
            PropertyName = propertyName;
            Starts = starts;
            Ends = ends;
        }

        public string PropertyName { get; }
        public object Starts { get; }
        public object Ends { get; }

        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
