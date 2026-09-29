namespace DataQI.Commons.Query
{
    public interface ICriteria
    {
        ICriteria Add(ICriterion criterion);
        ICriteria AddOrder(IOrderCriterion order);
    }
}
