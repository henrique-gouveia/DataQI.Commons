using System;
using System.Collections.Generic;

using ExpectedObjects;
using Xunit;

using DataQI.Commons.Repository;
using DataQI.Commons.Repository.Core;
using DataQI.Commons.Repository.Paging;
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
            Assert.Equal("The parameter should be an interface", exceptionMessage);          
        }

        [Fact]
        public void TestRejectInterfaceWithoutEntity()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => new RepositoryMetadata(typeof(IInvalidRepository)));

            Assert.Contains(nameof(IInvalidRepository), exception.Message);
            Assert.Contains("Could not resolve entity/id type", exception.Message);
        }

        [Theory]
        [InlineData(typeof(ICrudOnlyRepository))]
        [InlineData(typeof(IPagingOnlyRepository))]
        [InlineData(typeof(ICombinedRepository))]
        public void TestResolvesTheSharedMarkerForIndependentContracts(Type repository)
        {
            var metadata = new RepositoryMetadata(repository);

            Assert.Equal(typeof(FakeEntity), metadata.EntityType);
            Assert.Equal(typeof(int), metadata.IdType);
        }

        [Fact]
        public void TestRejectsDifferentEntityIdPairs()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => new RepositoryMetadata(typeof(IAmbiguousRepository)));

            Assert.Contains(nameof(IAmbiguousRepository), exception.Message);
            Assert.Contains("more than one entity/id pair", exception.Message);
        }

        [Fact]
        public void TestRejectsAnUnrelatedInterfaceWithACrudLikeName()
        {
            Assert.Throws<InvalidOperationException>(() => new RepositoryMetadata(typeof(INameOnlyRepository)));
        }

        public interface ICrudOnlyRepository : ICrudRepository<FakeEntity, int> { }
        public interface IPagingOnlyRepository : IPagedRepository<FakeEntity, int> { }
        public interface ICombinedRepository : ICrudRepository<FakeEntity, int>, IPagedRepository<FakeEntity, int> { }
        public interface IAmbiguousRepository : ICrudRepository<FakeEntity, int>, IPagedRepository<FakeEntity, long> { }
        public interface ICrudRepositoryLike<TEntity, TId> { }
        public interface INameOnlyRepository : ICrudRepositoryLike<FakeEntity, int> { }

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