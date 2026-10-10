namespace DataQI.Commons.Query
{
    public interface ICriterion
    {
        T Accept<T>(ICriterionVisitor<T> visitor);
    }
}
