namespace DataQI.Commons.Test.Repository.Query
{
    public sealed class QueryContractCase
    {
        public QueryContractCase(string methodName, string description, object[] arguments,
            bool expectsException = false, string expectedExceptionMessage = null)
        {
            MethodName = methodName;
            Description = description;
            Arguments = arguments;
            ExpectsException = expectsException;
            ExpectedExceptionMessage = expectedExceptionMessage;
        }

        public string MethodName { get; }
        public string Description { get; }
        public object[] Arguments { get; }
        public bool ExpectsException { get; }
        public string ExpectedExceptionMessage { get; }

        public override string ToString() => MethodName;
    }
}
