using System;
using Xunit;
using Moq;
using DataQI.Commons.Repository.Core;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Core
{
    public class RepositoryFactoryTest
    {
        private readonly Mock<IFakeRepository> customImplementationMock;
        private readonly RepositoryFactory repositoryFactory;

        public RepositoryFactoryTest()
        {
            customImplementationMock = new Mock<IFakeRepository>();
            repositoryFactory = new FakeRepositoryFactory(customImplementationMock.Object);
        }

        [Fact]
        public void TestRejectsNullCustomImplementation()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new FakeRepositoryFactory(null).GetRepository<IFakeRepository>());
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Custom Repository Implementation must not be null", exceptionMessage);
        }

        [Fact]
        public void TestGetRepositoryCorrectly()
        {
            var repository = repositoryFactory.GetRepository<IFakeRepository>();
            Assert.NotNull(repository);
        }


        [Fact]
        public void TestGetRepositoryMetadataCorrectly()
        {
            var repositoryMetadata = repositoryFactory.GetRepositoryMetadata<IFakeRepository>();

            Assert.NotNull(repositoryMetadata);
            Assert.Equal(typeof(int), repositoryMetadata.IdType);
            Assert.Equal(typeof(FakeEntity), repositoryMetadata.EntityType);
        }

        private class FakeRepositoryFactory : RepositoryFactory
        {
            private readonly object customImplementation;

            public FakeRepositoryFactory(object customImplementation)
                => this.customImplementation = customImplementation;

            protected override object GetCustomImplementation(Type repositoryInterface)
                => customImplementation;
        }
    }
}
