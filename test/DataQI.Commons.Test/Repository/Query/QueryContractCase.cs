namespace DataQI.Commons.Test.Repository.Query
{
    public sealed class QueryContractCase
    {
        public QueryContractCase(string methodName, string description, bool expectsException, string expectedExceptionMessage)
        {
            MethodName = methodName;
            Description = description;
            ExpectsException = expectsException;
            ExpectedExceptionMessage = expectedExceptionMessage;
        }

        public string MethodName { get; }
        public string Description { get; }
        public bool ExpectsException { get; }
        public string ExpectedExceptionMessage { get; }
    }
}
