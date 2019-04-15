namespace Net.Data.Commons.Criterions
{
    public interface IJunction : ICriterion
    {
         IJunction Add(ICriterion criterion);
    }
}