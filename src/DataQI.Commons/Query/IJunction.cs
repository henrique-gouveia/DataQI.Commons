namespace DataQI.Commons.Query
{
    public interface IJunction : ICriterion
    {
        IJunction Add(ICriterion criterion);
    }
}