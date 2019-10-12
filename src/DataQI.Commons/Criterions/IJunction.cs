namespace DataQI.Commons.Criterions
{
    public interface IJunction : ICriterion
    {
         IJunction Add(ICriterion criterion);
    }
}