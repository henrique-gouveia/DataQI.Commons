using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Net.Data.Commons.Util;

namespace Net.Data.Commons.Criteria.Support
{
    public class Criteria<TEntity> : ICriteria<TEntity> where TEntity : class, new()
    {
        private List<ICriterion> criterions = new List<ICriterion>();

        public object[] Parameters { get; set; }

        public ICriteria<TEntity> Add(ICriterion criterion)
        {
            criterions.Add(criterion);
            return this;
        }

        public string ToSqlString()
        {
            var sqlWhereBuilder = new StringBuilder();
            var enumerator = criterions.GetEnumerator();

            while (enumerator.MoveNext()) 
            {
                if (sqlWhereBuilder.Length > 0)
                    sqlWhereBuilder.Append(" AND ");
                
                var criterion = enumerator.Current;
                sqlWhereBuilder.Append(criterion.ToSqlString());
            }

            return sqlWhereBuilder.ToString();
        }
    }
}