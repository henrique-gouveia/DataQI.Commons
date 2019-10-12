using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using DataQI.Commons.Util;

namespace DataQI.Commons.Criterions.Support
{
    public class Criteria : ICriteria
    {
        private List<ICriterion> criterions = new List<ICriterion>();

        public ICriteria Add(ICriterion criterion)
        {
            criterions.Add(criterion);
            return this;
        }

        public ICriteria WithParameters(dynamic parameters)
        {
            Parameters = parameters;
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

        public object Parameters { get; private set; }
    }
}