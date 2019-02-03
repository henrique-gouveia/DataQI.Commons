using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Net.Data.Commons.Repository.Query
{
    public class Criterion
    {
        private readonly string source;

        public Criterion(string source)
        {
            this.source = source;
        }
    }
}