using System;

namespace Net.Data.Commons.Repository.Query
{
    public class Criterion
    {
        private readonly string source;
        private readonly CriterionTypeData type;

        public Criterion(string source)
        {
            this.source = source;
            
        }
    }
}