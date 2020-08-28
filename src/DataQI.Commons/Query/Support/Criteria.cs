using System.Collections.ObjectModel;
using System.Collections.Generic;

using DataQI.Commons.Util;

namespace DataQI.Commons.Query.Support
{
    public class Criteria : ICriteria
    {
        protected readonly IList<ICriterion> criterions = new List<ICriterion>();

        public ICriteria Add(ICriterion criterion)
        {
            Assert.NotNull(criterion, "Criterion must not be null");

            criterions.Add(criterion);
            return this;
        }

        public IReadOnlyCollection<ICriterion> Criterions => new ReadOnlyCollection<ICriterion>(criterions);
    }
}