using System;
using System.Collections.Generic;
using Bogus;
using ExpectedObjects;
using Moq;
using Net.Data.Commons.Repository;
using Net.Data.Commons.Repository.Core;
using Net.Data.Commons.Test.Repository.Sample;
using Xunit;

namespace Net.Data.Commons.Test.Repository.Core
{
    public class RepositoryMetadataTest
    {
        private static readonly Faker faker = new Faker();
        private readonly Mock<IFakeRepository> fakeRepositoryMock;
        private readonly IFakeRepository fakeRepository;

        public RepositoryMetadataTest()
        {
            fakeRepositoryMock = new Mock<IFakeRepository>();
            fakeRepository = RepositoryProxy.Create<IFakeRepository>(() => 
                fakeRepositoryMock.Object);
        }

        [Fact]
        public void TestExtractRepositoryMetadataCorrectly()
        {
            var repositoryMetadata = new RepositoryMetadata(typeof(IFakeRepository));
            var domainType = repositoryMetadata.EntityType;
            var typeId = repositoryMetadata.TypeId;
            AssertExpectedObject(typeof(FakeEntity), domainType);
            AssertExpectedObject(typeof(int), typeId);
        }

        [Fact]
        public void TestExtractRepositoryMetadataDefaultCorrectly()
        {
            var repositoryMetadata = new RepositoryMetadata(typeof(ICrudRepository<FakeEntity, int>));
            var domainType = repositoryMetadata.EntityType;
            var typeId = repositoryMetadata.TypeId;
            AssertExpectedObject(typeof(FakeEntity), domainType);
            AssertExpectedObject(typeof(int), typeId);
        }

        [Fact]
        public void TestExtractRepositoryMetadataCustomCorrectly()
        {
            var repositoryMetadata = new RepositoryMetadata(typeof(ICustomFakeRepository<FakeEntity, int>));
            var domainType = repositoryMetadata.EntityType;
            var typeId = repositoryMetadata.TypeId;
            AssertExpectedObject(typeof(FakeEntity), domainType);
            AssertExpectedObject(typeof(int), typeId);
        }

        [Fact]
        public void TestRejectNotInterfaceRepository()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new RepositoryMetadata(typeof(Object)));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("The parameter should be interface.", exceptionMessage);          
        }

        private void AssertExpectedObject(object expected, object actual)
        {
            expected.ToExpectedObject().ShouldEqual(actual);
        }

        public interface ICustomFakeRepository<TEntity, TId> where TEntity : class, new()
        {
            IEnumerable<FakeEntity> FindByName(string name);
        }
    }
}