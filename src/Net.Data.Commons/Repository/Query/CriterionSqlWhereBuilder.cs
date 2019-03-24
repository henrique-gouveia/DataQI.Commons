using System;
using System.Collections.Generic;
using System.Reflection;

using static Net.Data.Commons.Repository.Query.CriterionExtractor;

using Net.Data.Commons.Util;

namespace Net.Data.Commons.Repository.Query
{
    public class CriterionSqlWhereBuilder
    {
        private readonly IEnumerator<OrCriterion> orCriterions;

        private readonly ParameterInfo[] methodParameters;

        public CriterionSqlWhereBuilder(IEnumerator<OrCriterion> orCriterions, ParameterInfo[] methodParameters)
        {
            Assert.NotNull(orCriterions, "Criterions must not be null");
            Assert.NotNull(methodParameters, "Method Parameters must not be null");

            this.orCriterions = orCriterions;
            this.methodParameters = methodParameters;
        }
    }
}