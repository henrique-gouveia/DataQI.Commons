using System;

namespace Net.Data.Commons.Repository.Query
{
    public class CriterionExtractor
    {
        private readonly string source;

        public CriterionExtractor(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentException("Source must not be null");

            this.source = source;
        }

        public string[] Criterions => new string[] { source };
    }
}