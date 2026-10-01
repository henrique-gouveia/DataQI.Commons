namespace DataQI.Commons.Query.Ast
{
    public class Not : ICriterion
    {
        public Not(ICriterion inner)
        {
            Inner = inner;
        }

        public ICriterion Inner { get; }

        public T Accept<T>(ICriterionVisitor<T> visitor) => visitor.Visit(this);
    }
}
