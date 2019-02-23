using System;

namespace Net.Data.Commons.Repository.Query
{
    public class Criterion
    {
        public Criterion(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentException("Criterion source must not be null or empty");

            Type = CriterionTypeHelper.FromProperty(source);
            PropertyName = CriterionTypeHelper.ExtractProperty(source, Type);
        }

        public string PropertyName { get; private set; }

        public CriterionType Type { get; private set; }
    }
}