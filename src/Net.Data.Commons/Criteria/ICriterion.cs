using System.Collections.Generic;

namespace Net.Data.Commons.Criteria
{
    public interface ICriterion
    {
        string GetWhereOperator();

        string ToSqlString();
    }
}