using System.Collections.Generic;

namespace DataQI.Commons.Query
{
    public interface ICriteria
    {
        ICriteria Add(ICriterion criterion);
        ICriteria AddOrder(IOrderCriterion order);
        IReadOnlyCollection<ICriterion> Criterions { get; }
        IReadOnlyCollection<IOrderCriterion> Orders { get; }
    }
}
