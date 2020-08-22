using System.Collections.ObjectModel;
using System.Collections.Generic;

using DataQI.Commons.Util;

namespace DataQI.Commons.Query.Support
{
    public abstract class Junction : IJunction
    {
        protected readonly IList<ICriterion> criterions = new List<ICriterion>();

        public IJunction Add(ICriterion criterion)
        {
            Assert.NotNull(criterion, "Criterion must not be null");
            
            criterions.Add(criterion);
            return this;
        }

        public string GetPropertyName() => string.Empty;

        public abstract WhereOperator GetWhereOperator();

        public IReadOnlyCollection<ICriterion> Criterions => new ReadOnlyCollection<ICriterion>(criterions);
    }
}