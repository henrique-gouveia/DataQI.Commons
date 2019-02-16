using System;

namespace Net.Data.Commons.Repository.Query
{
    public class Criterion
    {
        private readonly string source;

        public Criterion(string source)
        {
            this.source = source;
            Data = new CriterionTypeData(source);
            PropertyName = Data.ExtractPropertyFromCriterionType();
        }

        public CriterionTypeData Data { get; private set; }

        public string PropertyName { get; set; }
    }
}