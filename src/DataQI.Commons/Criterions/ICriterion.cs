using System.Collections.Generic;

namespace DataQI.Commons.Criterions
{
    public interface ICriterion
    {
        string GetWhereOperator();

        string ToSqlString();
    }
}