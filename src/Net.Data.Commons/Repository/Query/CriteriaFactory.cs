using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

using Net.Data.Commons.Criterions;
using Net.Data.Commons.Criterions.Support;
using Net.Data.Commons.Util;

using static Net.Data.Commons.Repository.Query.CriterionExtractor;

namespace Net.Data.Commons.Repository.Query
{
    public class CriteriaFactory
    {
        private readonly MethodInfo queryMethod;

        private readonly object[] queryMethodArgs;

        public CriteriaFactory(MethodInfo queryMethod, object[] queryMethodArgs)
        {
            Assert.NotNull(queryMethod, "Query Method must not be null");
            Assert.NotNull(queryMethodArgs, "Query Method Parameters must not be null");

            this.queryMethod = queryMethod;
            this.queryMethodArgs = queryMethodArgs;
        }


        public ICriteria Create()
        {
            var criteria = new Criteria();
            BuildCriteria(criteria);

            return criteria;
        }

        public void BuildCriteria(ICriteria criteria)
        {
            AddCriterions(criteria);
            AddParameters(criteria);
        }

        private void AddCriterions(ICriteria criteria)
        {
            var extractor = new CriterionExtractor(queryMethod.Name);
            var orCriterions = extractor.GetEnumerator();

            while (orCriterions.MoveNext())
            {
                var orCriterion = orCriterions.Current;
                criteria.Add(CreateDisjunction(orCriterion));
            }
        }

        private void AddParameters(ICriteria criteria)
        {
            criteria.WithParameters(CreateCriteriaParameters());
        }

        private IJunction CreateDisjunction(OrCriterion orCriterion)
        {
            var disjunction = Restrictions.Disjuction();

            var andCriterions = orCriterion.GetEnumerator();
            while (andCriterions.MoveNext()) 
            {
                var andCriterion = andCriterions.Current;
                var andCriterionParameters = new List<string>();
                disjunction.Add(CreateConjunction(andCriterion));
            }

            return disjunction;
        }

        private IJunction CreateConjunction(Criterion andCriterion)
        {
            var conjunction = Restrictions.Conjuction();
            var andCriterionParameters = CreateCriterionParameters(andCriterion.Type);

            conjunction.Add(Restrictions
                .CreateCriterion(
                    andCriterion.PropertyName, 
                    andCriterion.Type, 
                    andCriterionParameters)
                );

            return conjunction;
        }

        private string[] CreateCriterionParameters(CriterionType type)
        {
            var criterionParameters = new List<string>();
            
            var parameters = queryMethod.GetParameters();
            int parameterIndex = 0;

            for (var i = 0; i < type.NumberOfArgs(); i++)
                criterionParameters.Add($"@{parameters[parameterIndex++].Name}");

            return criterionParameters.ToArray();
        }

        private dynamic CreateCriteriaParameters()
        {
            dynamic parameters = new ExpandoObject();
            var parametersDictionary = (IDictionary<string, object>) parameters;

            var methodParameters = queryMethod.GetParameters();
            for (var i = 0; i < methodParameters.Length; i++) 
                parametersDictionary.Add(methodParameters[i].Name, queryMethodArgs[i]);

            return parameters;
        }
    }
}