using System;
using System.Collections.Generic;
using Bogus;
using Moq;
using Net.Data.Commons.Repository;
using Net.Data.Commons.Repository.Core;
using Xunit;

namespace Net.Data.Commons.Test.Repository.Core
{
    public class RepositoryMetadataTest
    {
        private static readonly Faker faker = new Faker();
        private readonly Mock<IFakeRepository> fakeRepositoryMock;
        private readonly IFakeRepository fakeRepository;

        //private readonly IRepositoryMetadata repositoryMetadata;

        public RepositoryMetadataTest()
        {
            fakeRepositoryMock = new Mock<IFakeRepository>();
            fakeRepository = RepositoryProxy.Create<IFakeRepository>(() => 
                fakeRepositoryMock.Object);
        }

        [Fact]
        public void TestExtractRepositoryMetadataSuccessfully()
        {
            var repositoryMetadata = new RepositoryMetadata(typeof(IFakeRepository));
            var domainType = repositoryMetadata.EntityType;
            var typeId = repositoryMetadata.TypeId;
            Assert.Equal(typeof(FakeEntity), domainType);
            Assert.Equal(typeof(int), typeId);
        }

        [Fact]
        public void TestExtractRepositoryMetadataDefaultSuccessfully()
        {
            var repositoryMetadata = new RepositoryMetadata(typeof(ICrudRepository<FakeEntity, int>));
            var domainType = repositoryMetadata.EntityType;
            var typeId = repositoryMetadata.TypeId;
            Assert.Equal(typeof(FakeEntity), domainType);
            Assert.Equal(typeof(int), typeId);
        }

        [Fact]
        public void TestExtractRepositoryMetadataCustomSuccessfully()
        {
            var repositoryMetadata = new RepositoryMetadata(typeof(ICustomFakeRepository<FakeEntity, int>));
            var domainType = repositoryMetadata.EntityType;
            var typeId = repositoryMetadata.TypeId;
            Assert.Equal(typeof(FakeEntity), domainType);
            Assert.Equal(typeof(int), typeId);
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

        public interface IFakeRepository : ICrudRepository<FakeEntity, int>
        {
            IEnumerable<FakeEntity> FindByName(string name);
        }

        public interface ICustomFakeRepository<TEntity, TId> where TEntity : class, new()
        {
            IEnumerable<FakeEntity> FindByName(string name);
        }

        public class FakeEntity
        {
            public FakeEntity()
            {
            }

            public FakeEntity(int id) : this(id, null)
            {
            }

            public FakeEntity(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}