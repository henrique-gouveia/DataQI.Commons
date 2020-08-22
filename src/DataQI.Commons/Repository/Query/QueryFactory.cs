using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using DataQI.Commons.Extensions.Collections;
using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Util;

using static DataQI.Commons.Repository.Query.QueryTree;

namespace DataQI.Commons.Repository.Query
{
    public class QueryFactory
    {
        private readonly QueryTree queryTree;
        private readonly IEnumerator queryValues;

        public QueryFactory(MethodInfo queryMethod, object[] queryValues)
        {
            Assert.NotNull(queryMethod, "Query Method must not be null");
            Assert.NotNull(queryValues, "Query Values must not be null");

            this.queryTree = new QueryTree(queryMethod.Name);
            this.queryValues = queryValues.GetEnumerator();
        }

        public ICriteria CreateCriteria()
        {
            ICriteria criteria = new Criteria();
            BuildCriteria(criteria);

            return criteria;
        }

        public void BuildCriteria(ICriteria criteria)
        {
            Assert.NotNull(criteria, "Criteria must be not null");
            BuildOr(criteria, queryTree.GetEnumerator(), queryValues);
        }

        private void BuildOr(ICriteria criteria, IEnumerator<Node> nodes, IEnumerator values)
        {
            var or = new Disjunction();
            
            while (nodes.MoveNext())
                BuildAnd(or, nodes.Current.GetEnumerator(), values);
            
            criteria.Add(or);
        }

        private void BuildAnd(IJunction or, IEnumerator<QueryMember> members, IEnumerator values)
        {
            var and = new Conjunction();

            while (members.MoveNext())
            {
                var criterion = BuildCriterion(members.Current, values);
                and.Add(criterion);
            }

            or.Add(and);
        }

        private ICriterion BuildCriterion(QueryMember member, IEnumerator values)
        {
            ICriterion criterion;

            WhereOperator wo;
            if (!Enum.TryParse(member.Type.ToString(), out wo))
                wo = WhereOperator.Equal;            

            switch (wo)
            {
                case WhereOperator.Between:
                    criterion = new BetweenExpression(member.PropertyName, values.NextValue(), values.NextValue());
                    break;
                case WhereOperator.In:
                    criterion = new InExpression(member.PropertyName, values.NextValue<object[]>());
                    break;
                case WhereOperator.Null:
                    criterion = new NullExpression(member.PropertyName);
                    break;
                default: 
                    criterion = new SimpleExpression(member.PropertyName, wo, values.NextValue());
                    break;
            }

            if (member.HasNot)
                criterion = new NotExpression(criterion);

            return criterion;
        }
    }
}