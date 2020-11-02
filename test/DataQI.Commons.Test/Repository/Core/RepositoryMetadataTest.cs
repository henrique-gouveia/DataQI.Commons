using System;
using System.Collections.Generic;

using ExpectedObjects;
using Xunit;

using DataQI.Commons.Repository;
using DataQI.Commons.Repository.Core;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Core
{
    public class RepositoryMetadataTest
    {
        [Fact]
        public void TestExtractRepositoryMetadataCorrectly()
        {
            var repositoryMetadata = new RepositoryMetadata(typeof(IFakeRepository));
            var domainType = repositoryMetadata.EntityType;
            var typeId = repositoryMetadata.IdType;
            AssertExpectedObject(typeof(FakeEntity), domainType);
            AssertExpectedObject(typeof(int), typeId);
        }

        [Fact]
        public void TestExtractRepositoryMetadataDefaultCorrectly()
        {
            var repositoryMetadata = new RepositoryMetadata(typeof(ICrudRepository<FakeEntity, int>));
            var domainType = repositoryMetadata.EntityType;
            var typeId = repositoryMetadata.IdType;
            AssertExpectedObject(typeof(FakeEntity), domainType);
            AssertExpectedObject(typeof(int), typeId);
        }

        [Fact]
        public void TestExtractRepositoryMetadataCustomCorrectly()
        {
            var repositoryMetadata = new RepositoryMetadata(typeof(ICustomFakeRepository<FakeEntity, int>));
            var domainType = repositoryMetadata.EntityType;
            var typeId = repositoryMetadata.IdType;
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
            Assert.Equal("The parameter should be interface", exceptionMessage);          
        }

        [Fact]
        public void TestRejectInterfaceWithoutEntity()
        {
            var exception = Assert.Throws<ArgumentException>(() => new RepositoryMetadata(typeof(IFakeInvalidRepository)));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Could not resolve entity type", exceptionMessage);
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