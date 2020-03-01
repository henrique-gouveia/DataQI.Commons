using System.Collections.Generic;
using System.Text;

namespace DataQI.Commons.Criterions.Support
{
    public abstract class Junction : IJunction
    {
        private readonly List<ICriterion> criterions = new List<ICriterion>();

        public IJunction Add(ICriterion criterion)
        {
            criterions.Add(criterion);
            return this;
        }
        
        public abstract string GetWhereOperator();

        public string ToSqlString()
        {
            var sqlWhereBuilder = new StringBuilder();
            var enumerator = criterions.GetEnumerator();

            while (enumerator.MoveNext()) 
            {
                if (sqlWhereBuilder.Length > 0)
                    sqlWhereBuilder.Append(GetWhereOperator());
                
                var criterion = enumerator.Current;
                var sqlFormat = criterion is IJunction ? "({0})" : "{0}";
                
                sqlWhereBuilder.AppendFormat(sqlFormat, criterion.ToSqlString());
            }

            return sqlWhereBuilder.ToString();
        }
    }
}