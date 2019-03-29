using System;
using System.Collections.Generic;
using System.Reflection;

using Net.Data.Commons.Criterions;
using Net.Data.Commons.Criterions.Support;
using Net.Data.Commons.Util;

using static Net.Data.Commons.Repository.Query.CriterionExtractor;

namespace Net.Data.Commons.Repository.Query
{
    public class CriteriaFactory
    {
        private readonly IEnumerator<OrCriterion> orCriterions;

        private readonly ParameterInfo[] methodParameters;

        public CriteriaFactory(IEnumerator<OrCriterion> orCriterions, ParameterInfo[] methodParameters)
        {
            Assert.NotNull(orCriterions, "Criterions must not be null");
            Assert.NotNull(methodParameters, "Method Parameters must not be null");

            this.orCriterions = orCriterions;
            this.methodParameters = methodParameters;
        }


        public ICriteria Create()
        {
            var criteria = new Criteria();
            
            while (orCriterions.MoveNext())
            {
                var orCriterion = orCriterions.Current;
                criteria.Add(CreateDisjunction(orCriterion));
            }
            
            return criteria;
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
            int parameterIndex = 0;

            for (var i = 0; i < type.NumberOfArgs(); i++)
                criterionParameters.Add($"@{methodParameters[parameterIndex++].Name}");

            return criterionParameters.ToArray();
        }
    }
}