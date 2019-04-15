using System.Collections.Generic;

namespace Net.Data.Commons.Criterions
{
    public interface ICriterion
    {
        string GetWhereOperator();

        string ToSqlString();
    }
}