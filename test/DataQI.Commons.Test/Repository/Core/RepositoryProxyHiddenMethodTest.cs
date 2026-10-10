using DataQI.Commons.Repository;
using DataQI.Commons.Repository.Core;
using DataQI.Commons.Test.Repository.Sample;

using Xunit;

namespace DataQI.Commons.Test.Repository.Core
{
    public class RepositoryProxyHiddenMethodTest
    {
        public interface IEchoRepository : IEntityRepository<FakeEntity>
        {
            string Echo(string value);
        }

        public class EchoBase
        {
            public string Echo(string value) => value;
        }

        public class EchoHiding : EchoBase
        {
            public new string Echo(string value) => $"{value}!";
        }

        [Fact]
        public void TestAMethodHiddenByNewIsRegisteredOnce()
        {
            var repository = RepositoryProxy<IEchoRepository>.Create(() => new EchoHiding());

            Assert.Equal("x!", repository.Echo("x"));
        }
    }
}
