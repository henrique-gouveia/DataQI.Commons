namespace Net.Data.Commons.Criteria
{
    public interface IJunction : ICriterion
    {
         IJunction Add(ICriterion criterion);
    }
}