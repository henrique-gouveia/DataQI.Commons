using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Bogus;
using ExpectedObjects;
using Moq;
using Xunit;

using Net.Data.Commons.Criterions;
using Net.Data.Commons.Criterions.Support;
using Net.Data.Commons.Repository;
using Net.Data.Commons.Repository.Core;
using Net.Data.Commons.Test.Repository.Sample;

namespace Net.Data.Commons.Test.Repository.Core
{
    public class RepositoryProxyTest
    {
        private static readonly Faker faker = new Faker();
        private readonly Mock<IFakeRepository> fakeRepositoryMock;
        private readonly IFakeRepository fakeRepository;

        public RepositoryProxyTest()
        {
            fakeRepositoryMock = new Mock<IFakeRepository>();
            fakeRepository = RepositoryProxy.Create<IFakeRepository>(() => 
                fakeRepositoryMock.Object);
        }

        [Fact]
        public void TestRejectsNullRepository()
        {
            var exception = Assert.Throws<TargetInvocationException>(() => 
                RepositoryProxy.Create<ICrudRepository<Object, int>>(() => null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Repository must not be null", exceptionMessage);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestInvokeInsertCorrectly(bool useAsyncMethod)
        {
            var entityExpected = CreateTestFakeEntity();
            SetupFakeRepositoryInsertMethod(fe => fe.Id = entityExpected.Id, useAsyncMethod);

            var entity = new FakeEntity() { Name = entityExpected.Name };
            if (useAsyncMethod)
                fakeRepository.InsertAsync(entity).GetAwaiter().GetResult();
            else
                fakeRepository.Insert(entity);

            AssertExpectedObject(entityExpected, entity);
        }
        
        private void SetupFakeRepositoryInsertMethod(Action<FakeEntity> callback, bool useAsyncMethod)
        {
            if (useAsyncMethod)
            {
                fakeRepositoryMock
                    .Setup(r => r.InsertAsync(It.IsAny<FakeEntity>()))
                    .Callback<FakeEntity>(callback)
                    .Returns(Task.FromResult(0));
            }
            else 
            {
                fakeRepositoryMock
                    .Setup(r => r.Insert(It.IsAny<FakeEntity>()))
                    .Callback<FakeEntity>(callback);
            }
        }

        [Theory]
        [InlineData(0, "Inserted", false)]
        [InlineData(1, "Updated", false)]
        [InlineData(0, "Inserted", true)]
        [InlineData(1, "Updated", true)]
        public void TestInvokeSaveMethodCorrectly(int fakeEntityId, string fakeEntityName, bool useAsyncMethod)
        {
            FakeEntity entityExpected = null;
            SetupFakeRepositorySaveMethod(fe => 
                {
                    var isNew = fe.Id == 0;
                    if (isNew)
                    {
                        entityExpected = CreateTestFakeEntity(null, fe.Name);
                        fe.Id = entityExpected.Id;
                    }
                    else
                        entityExpected = CreateTestFakeEntity(fe.Id, fe.Name);
                },
                useAsyncMethod);

            var entity = CreateTestFakeEntity(fakeEntityId, fakeEntityName);
            if (useAsyncMethod)
                fakeRepository.SaveAsync(entity).GetAwaiter().GetResult();
            else
                fakeRepository.Save(entity);

            AssertExpectedObject(entityExpected, entity);
        }
        private void SetupFakeRepositorySaveMethod(Action<FakeEntity> callback, bool useAsyncMethod)
        {
            if (useAsyncMethod)
            {
                fakeRepositoryMock
                    .Setup(r => r.SaveAsync(It.IsAny<FakeEntity>()))
                    .Callback<FakeEntity>(callback)
                    .Returns(Task.FromResult(0));
            }
            else
            {
                fakeRepositoryMock
                    .Setup(r => r.Save(It.IsAny<FakeEntity>()))
                    .Callback<FakeEntity>(callback);
            }
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public void TestInvokeExistsCorrectly(bool returnsExists, bool useAsyncMethod)
        {
            var entityExpected = CreateTestFakeEntity();
            SetupFakeRepositoryExistsMethod(entityExpected.Id, returnsExists, useAsyncMethod);

            bool exists;
            if (useAsyncMethod)
                exists = fakeRepository.ExistsAsync(entityExpected.Id).Result;
            else
                exists = fakeRepository.Exists(entityExpected.Id);
            
            Assert.Equal(returnsExists, exists);
        }

        private void SetupFakeRepositoryExistsMethod(int fakeEntityId, bool returnsExists, bool useAsyncMethod)
        {
            if (useAsyncMethod)
            {
                fakeRepositoryMock
                    .Setup(r => r.ExistsAsync(fakeEntityId))
                    .Returns(Task.FromResult(returnsExists));
            }
            else
            {
                fakeRepositoryMock
                    .Setup(r => r.Exists(fakeEntityId))
                    .Returns(returnsExists);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestInvokeFindCorrectly(bool useAsyncMethod)
        {
            var entitiesExpected = CreateTestFakeEntities();
            var criteria = new Criteria()
                .Add(Restrictions.Equal("Name", "@name"));
            SetupFakeRepositoryFindMethod(criteria, entitiesExpected, useAsyncMethod);

            IEnumerable<FakeEntity> entities;
            if (useAsyncMethod)
                entities = fakeRepository.FindAsync(criteria).Result;
            else
                entities = fakeRepository.Find(criteria);

            AssertExpectedObject(entitiesExpected, entities);
        }

        private void SetupFakeRepositoryFindMethod(ICriteria criteria, IEnumerable<FakeEntity> returnsFakeEntities, bool useAsyncMethod)
        {
            if (useAsyncMethod)
            {
                fakeRepositoryMock
                    .Setup(r => r.FindAsync(criteria))
                    .Returns(Task.FromResult<IEnumerable<FakeEntity>>(returnsFakeEntities));
            }
            else
            {
                fakeRepositoryMock
                    .Setup(r => r.Find(criteria))
                    .Returns(returnsFakeEntities);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestInvokeFindAllCorrectly(bool useAsyncMethod)
        {
            var entitiesExpected = CreateTestFakeEntities();
            SetupFakeRepositoryFindAllMethod(entitiesExpected, useAsyncMethod);

            IEnumerable<FakeEntity> entities;
            if (useAsyncMethod)
                entities = fakeRepository.FindAllAsync().Result;
            else
                entities = fakeRepository.FindAll();

            AssertExpectedObject(entitiesExpected, entities);
        }

        private void SetupFakeRepositoryFindAllMethod(IEnumerable<FakeEntity> returnsFakeEntities, bool useAsyncMethod)
        {
            if (useAsyncMethod)
            {
                fakeRepositoryMock
                    .Setup(r => r.FindAllAsync())
                    .Returns(Task.FromResult<IEnumerable<FakeEntity>>(returnsFakeEntities));
            }
            else
            {
                fakeRepositoryMock
                    .Setup(r => r.FindAll())
                    .Returns(returnsFakeEntities);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestInvokeFindOneCorrectly(bool useAsyncMethod) 
        {
            var entityExpected = CreateTestFakeEntity();
            SetupFakeRepositoryFindOneMethod(entityExpected, useAsyncMethod);

            FakeEntity entity;
            if (useAsyncMethod)
                entity = fakeRepository.FindOneAsync(entityExpected.Id).Result;
            else
                entity = fakeRepository.FindOne(entityExpected.Id);
            
            AssertExpectedObject(entityExpected, entity);
        }

        private void SetupFakeRepositoryFindOneMethod(FakeEntity returnsFakeEntity, bool useAsyncMethod)
        {
            if (useAsyncMethod)
            {
                fakeRepositoryMock
                    .Setup(r => r.FindOneAsync(returnsFakeEntity.Id))
                    .Returns(Task.FromResult<FakeEntity>(returnsFakeEntity));
            }
            else
            {
                fakeRepositoryMock
                    .Setup(r => r.FindOne(returnsFakeEntity.Id))
                    .Returns(returnsFakeEntity);
            }
        }

        [Theory]
        [InlineData(false)] 
        [InlineData(true)] 
        public void TestInvokeDeleteCorrectly(bool useAsyncMethod) 
        {
            var entityExpected = CreateTestFakeEntity();

            if (useAsyncMethod)
            {
                fakeRepository.Delete(entityExpected.Id);
                fakeRepositoryMock.Verify(r => r.Delete(entityExpected.Id), Times.Once());
            }
            else
            {
                fakeRepository.DeleteAsync(entityExpected.Id).GetAwaiter().GetResult();
                fakeRepositoryMock.Verify(r => r.DeleteAsync(entityExpected.Id), Times.Once());
            }
        }

        [Fact]
        public void TestInvokeNonDefaultFindMethodCorrectly()
        {
            var entityExpected = CreateTestFakeEntity();
            var entitiesExpected = new List<FakeEntity>() { entityExpected };

            fakeRepositoryMock
                .Setup(r => r.Find(It.IsAny<Criteria>()))
                .Returns(entitiesExpected);

            var entities = fakeRepository.FindByName(entityExpected.Name);

            AssertExpectedObject(entitiesExpected, entities);
        }

        private void AssertExpectedObject(object expected, object actual)
        {
            expected.ToExpectedObject().ShouldEqual(actual);
        }

        private IList<FakeEntity> CreateTestFakeEntities()
        {
            return new List<FakeEntity>()
            {
                CreateTestFakeEntity(),
                CreateTestFakeEntity(),
                CreateTestFakeEntity(),
                CreateTestFakeEntity(),
                CreateTestFakeEntity()
            };
        }

        private FakeEntity CreateTestFakeEntity(int? fakeEntityId = null, string fakeEntityName = null)
        {
            var id = fakeEntityId.HasValue ? fakeEntityId.Value : faker.Random.Int(0, 100);
            var name = fakeEntityName != null ? fakeEntityName : faker.Person.FullName;

            return new FakeEntity(id, name);
        }
    }
}