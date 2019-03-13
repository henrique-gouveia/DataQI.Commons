using System;
using Net.Data.Commons.Util;

namespace Net.Data.Commons.Repository.Query
{
    public class Criterion
    {
        public Criterion(string source)
        {
            Assert.IsNullOrEmpty(source, "Criterion source must not be null or empty");
            Type = CriterionTypeHelper.FromProperty(source);
            PropertyName = CriterionTypeHelper.ExtractProperty(source, Type);
        }

        public Criterion(string propertyName, CriterionType type) 
        {
            this.PropertyName = propertyName;
                this.Type = type;
               
        }
                public string PropertyName { get; private set; }

        public CriterionType Type { get; private set; }
    }
}