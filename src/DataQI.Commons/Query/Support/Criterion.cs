namespace DataQI.Commons.Query.Support
{
    public abstract class Criterion : ICriterion
    {
        private readonly string propertyName;
        private readonly WhereOperator whereOperator;

        internal Criterion(string propertyName, WhereOperator whereOperator)
        {
            this.propertyName = propertyName;
            this.whereOperator = whereOperator;
        }

        public string GetPropertyName() => propertyName;

        public WhereOperator GetWhereOperator() => whereOperator;
    }
}