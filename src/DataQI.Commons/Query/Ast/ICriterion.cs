namespace DataQI.Commons.Query.Ast
{
    public interface ICriterion
    {
        T Accept<T>(ICriterionVisitor<T> visitor);
    }
}
