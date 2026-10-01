using System.Collections.Generic;

namespace DataQI.Commons.Query
{
    public interface ICriteria
    {
        ICriteria Add(Ast.ICriterion criterion);
        ICriteria AddOrder(IOrderCriterion order);
        IReadOnlyCollection<Ast.ICriterion> Criterions { get; }
        IReadOnlyCollection<IOrderCriterion> Orders { get; }
    }
}
