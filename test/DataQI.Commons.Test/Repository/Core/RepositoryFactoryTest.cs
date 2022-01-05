using System;

using Moq;
using Xunit;

using DataQI.Commons.Repository.Core;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Core
{
    public class RepositoryFactoryTest
    {
        private readonly Mock<IFakeRepository> fakeRepositoryMock;
        private readonly RepositoryFactory fakeRepositoryFactory;

        public RepositoryFactoryTest()
        {
            fakeRepositoryMock = new Mock<IFakeRepository>();
            fakeRepositoryFactory = new FakeRepositoryFactory(() => fakeRepositoryMock.Object);
        }

        [Fact]
        public void TestRejectsNullRepositoryInstance()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new FakeRepositoryFactory(() => null).GetRepository<IFakeRepository>());
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Repository instance must not be null", exceptionMessage);
        }

        [Fact]
        public void TestRejectsNullRepositoryFactory()
        {
            Func<object> respositoryFactory = null;

            var exception = Assert.Throws<ArgumentException>(() =>
                new FakeRepositoryFactory().GetRepository<IFakeRepository>(respositoryFactory));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Repository factory must not be null", exceptionMessage);
        }

        [Fact]
        public void TestGetRepositoryCorrectly()
        {
            var repository = fakeRepositoryFactory.GetRepository<IFakeRepository>();
            Assert.NotNull(repository);
        }

        [Fact]
        public void TestGetRepositoryMetadataCorrectly()
        {
            var repositoryMetadata = fakeRepositoryFactory.GetRepositoryMetadata<IFakeRepository>();

            Assert.NotNull(repositoryMetadata);
            Assert.Equal(typeof(int), repositoryMetadata.IdType);
            Assert.Equal(typeof(FakeEntity), repositoryMetadata.EntityType);
        }

        private class FakeRepositoryFactory : RepositoryFactory
        {
            private readonly Func<object> repositoryFactory;

            public FakeRepositoryFactory() : this(null)
            { }

            public FakeRepositoryFactory(Func<object> repositoryFactory)
                => this.repositoryFactory = repositoryFactory;

            protected override object GetRepositoryInstance(Type repositoryType, params object[] args)
                => repositoryFactory();
        }
    }
}
