using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

using DataQI.Commons.Criterions;
using DataQI.Commons.Criterions.Support;
using DataQI.Commons.Util;

using static DataQI.Commons.Repository.Query.CriterionExtractor;

namespace DataQI.Commons.Repository.Query
{
    public class CriteriaFactory
    {
        private readonly MethodInfo queryMethod;
        
        private readonly object[] queryMethodArgs;

        private readonly IEnumerator<ParameterInfo> queryMethodParameters;

        public CriteriaFactory(MethodInfo queryMethod, object[] queryMethodArgs)
        {
            Assert.NotNull(queryMethod, "Query Method must not be null");
            Assert.NotNull(queryMethodArgs, "Query Method Parameters must not be null");

            this.queryMethod = queryMethod;
            this.queryMethodArgs = queryMethodArgs;
            this.queryMethodParameters = queryMethod.GetParameters().Cast<ParameterInfo>().GetEnumerator();
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
            criteria.Add(CreateDisjunction(extractor.GetEnumerator()));
        }

        private void AddParameters(ICriteria criteria) => criteria.WithParameters(CreateCriteriaParameters());

        private IJunction CreateDisjunction(IEnumerator<OrCriterion> orCriterions)
        {
            var disjunction = Restrictions.Disjuction();

            while (orCriterions.MoveNext()) 
            {
                var orCriterion = orCriterions.Current;
                disjunction.Add(CreateConjunction(orCriterion.GetEnumerator()));
            }

            return disjunction;
        }

        private IJunction CreateConjunction(IEnumerator<Criterion> andCriterions)
        {
            var conjunction = Restrictions.Conjuction();

            while (andCriterions.MoveNext()) 
            {
                var andCriterion = andCriterions.Current;
                conjunction.Add(CreateCriterion(andCriterion));
            }

            return conjunction;
        }

        private ICriterion CreateCriterion(Criterion criterion)
        {
            var criterionParameters = CreateCriterionParameters(criterion.Type);
            return Restrictions.CreateCriterion(
                criterion.PropertyName, 
                criterion.Type, 
                criterionParameters);
        }

        private string[] CreateCriterionParameters(CriterionType type)
        {
            var criterionParameters = new List<string>();

            for (var i = 0; i < type.NumberOfArgs(); i++)
                if (queryMethodParameters.MoveNext() && queryMethodParameters.Current != null)
                    criterionParameters.Add($"@{queryMethodParameters.Current.Name}");

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