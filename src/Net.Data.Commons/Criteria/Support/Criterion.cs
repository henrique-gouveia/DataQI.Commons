using System.Linq;
using System;
using System.Collections.Generic;

using Net.Data.Commons.Util;

namespace Net.Data.Commons.Criteria.Support
{
    public class Criterion : ICriterion
    {
        public Criterion(string source)
        {
            Assert.IsNotNullOrEmpty(source, "Criterion source must not be null or empty");

            Type = CriterionTypeHelper.FromProperty(source);
            PropertyName = CriterionTypeHelper.ExtractProperty(source, Type);
        }

        public Criterion(string propertyName, CriterionType type) : 
            this(propertyName, type, Array.Empty<string>())
        {
        }

        public Criterion(string propertyName, CriterionType type, string[] parametersNames) 
        {
            this.PropertyName = propertyName;
            this.Type = type;
            this.ParametersNames = parametersNames;
        }

        public string GetWhereOperator()
        {
            return Type.ToString();
        }

        public string ToSqlString()
        {
            var parametersFormat = Array
                .Empty<string>()
                .Concat(new string[] { PropertyName })
                .Concat(ParametersNames)
                .ToArray();

            return string.Format(Type.CommandTemplate(), parametersFormat);
        }

        public string PropertyName { get; private set; }

        public CriterionType Type { get; private set; }

        public IReadOnlyCollection<string> ParametersNames { get; private set; }
    }
}