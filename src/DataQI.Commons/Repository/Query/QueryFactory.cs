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
            BuildOr(criteria, queryTree, queryValues);
        }

        private static void BuildOr(ICriteria criteria, IEnumerable<Node> nodes, IEnumerator values)
        {
            var or = new Disjunction();
            foreach (var node in nodes)
                BuildAnd(or, node, values);
            criteria.Add(or);
        }

        private static void BuildAnd(IJunction or, IEnumerable<QueryMember> members, IEnumerator values)
        {
            var and = new Conjunction();
            foreach (var member in members)
            {
                var criterion = BuildCriterion(member, values);
                and.Add(criterion);
            }
            or.Add(and);
        }

        private static ICriterion BuildCriterion(QueryMember member, IEnumerator values)
        {
            ICriterion criterion;

            if (!Enum.TryParse(member.Type.ToString(), out WhereOperator wo))
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
                case WhereOperator.Containing:
                case WhereOperator.EndingWith:
                case WhereOperator.Equal:
                case WhereOperator.GreaterThan:
                case WhereOperator.GreaterThanEqual:
                case WhereOperator.LessThan:
                case WhereOperator.LessThanEqual:
                case WhereOperator.Like:
                case WhereOperator.Not:
                case WhereOperator.StartingWith:
                case WhereOperator.And:
                case WhereOperator.Or:
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