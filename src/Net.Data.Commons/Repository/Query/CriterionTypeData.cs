using System;
using System.Collections.Generic;

namespace Net.Data.Commons.Repository.Query
{
    public class CriterionTypeData
    {
        private readonly string source;

        public CriterionTypeData(string source)
        {
            this.source = source;
            Type = ExtractTypeFromSource(source);
        }

        public string ExtractPropertyFromCriterionType()
        {
            if (Type == CriterionType.SimpleProperty)
                return source;
            else
                return source.Substring(0, source.IndexOf(Type.ToString()));
        }

        public CriterionType ExtractTypeFromSource(string source)
        {
            var types = Enum.GetValues(typeof(CriterionType));
            foreach (var type in types)
                if (source.EndsWith(type.ToString()))
                    return (CriterionType) type;

            return CriterionType.SimpleProperty;
        }

        public CriterionType Type { get; private set; }
    }
}