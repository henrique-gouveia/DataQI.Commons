using System;

namespace Net.Data.Commons.Repository.Query
{
    public class Criterion
    {
        private readonly string source;
        private readonly Type type;

        public Criterion(string source)
        {
            this.source = source;
            if (!Enum.TryParse(source, out this.type))
            {
                this.type = Type.SimpleProperty;
            }
        }
    }
}