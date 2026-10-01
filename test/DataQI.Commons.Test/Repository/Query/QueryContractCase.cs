namespace DataQI.Commons.Test.Repository.Query
{
    public sealed class QueryContractCase
    {
        public QueryContractCase(string methodName, string description, object[] arguments)
        {
            MethodName = methodName;
            Description = description;
            Arguments = arguments;
        }

        public string MethodName { get; }
        public string Description { get; }
        public object[] Arguments { get; }

        public override string ToString() => MethodName;
    }
}
