using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Bogus;
using ExpectedObjects;
using Moq;
using Net.Data.Commons.Repository;
using Net.Data.Commons.Repository.Core;
using Xunit;

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
        
        [Fact] 
        public void TestInvokeInsert() 
        {
            var entityExpected = CreateTestFakeEntity();
            fakeRepositoryMock
                .Setup(r => r.Insert(It.IsAny<FakeEntity>()))
                .Callback<FakeEntity>(fe => fe.Id = entityExpected.Id);

            var entity = new FakeEntity() { Name = entityExpected.Name };
            fakeRepository.Insert(entity);

            AssertExpectedObject(entityExpected, entity);
        }

        [Fact] 
        public async void TestInvokeInsertAsync()
        {
            var entityExpected = CreateTestFakeEntity();
            fakeRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<FakeEntity>()))
                .Callback<FakeEntity>(fe => fe.Id = entityExpected.Id)
                .Returns(Task.FromResult(0));

            var entity = new FakeEntity() { Name = entityExpected.Name };
            await fakeRepository.InsertAsync(entity);
            
            AssertExpectedObject(entityExpected, entity);
        }

        [Fact] 
        public void TestInvokeSave() 
        {
            var entityExpected = CreateTestFakeEntity();
            fakeRepositoryMock
                .Setup(r => r.Save(It.IsAny<FakeEntity>()))
                .Callback<FakeEntity>(fe => 
                {
                    var isNew = fe.Id == 0;
                    if (isNew)
                        fe.Id = entityExpected.Id;
                    else
                        entityExpected.Name = fe.Name;
                });
            
            var entity = new FakeEntity() { Name = entityExpected.Name };
            fakeRepository.Save(entity);
            AssertExpectedObject(entityExpected, entity);

            entity.Name = CreateTestFakeEntity().Name;
            fakeRepository.Save(entity);
            AssertExpectedObject(entityExpected, entity);
        }

        [Fact] 
        public async void TestInvokeSaveAsync()
        {
            var entityExpected = CreateTestFakeEntity();
            fakeRepositoryMock
                .Setup(r => r.SaveAsync(It.IsAny<FakeEntity>()))
                .Callback<FakeEntity>(fe => 
                {
                    var isNew = fe.Id == 0;
                    if (isNew)
                        fe.Id = entityExpected.Id;
                    else
                        entityExpected.Name = fe.Name;
                })
                .Returns(Task.FromResult(0));

            var entity = new FakeEntity() { Name = entityExpected.Name };
            await fakeRepository.SaveAsync(entity);
            AssertExpectedObject(entityExpected, entity);

            entity.Name = CreateTestFakeEntity().Name;
            await fakeRepository.SaveAsync(entity);
            AssertExpectedObject(entityExpected, entity);
        }

        [Fact] 
        public void TestInvokeExists() 
        {
            var entityExpected = CreateTestFakeEntity();
            fakeRepositoryMock
                .Setup(r => r.Exists(entityExpected.Id))
                .Returns(true);

            var exists = fakeRepository.Exists(entityExpected.Id);
            
            Assert.True(exists);
        }

        [Fact] 
        public async void TestInvokeExistsAsync() 
        {
            var entityExpected = CreateTestFakeEntity();
            fakeRepositoryMock
                .Setup(r => r.ExistsAsync(entityExpected.Id))
                .Returns(Task.FromResult(true));

            var exists = await fakeRepository.ExistsAsync(entityExpected.Id);
            
            Assert.True(exists);
        }

        [Fact]
        public void TestInvokeFind()
        {
            var entitiesExpected = CreateTestFakeEntities();
            fakeRepositoryMock
                .Setup(r => r.Find(It.IsAny<FormattableString>(), It.IsAny<object>()))
                .Returns(entitiesExpected);

            var entities = fakeRepository.Find($"Name = @name", new object());

            AssertExpectedObject(entitiesExpected, entities);
        }

        [Fact]
        public async void TestInvokeFindAsync()
        {
            var entitiesExpected = CreateTestFakeEntities();
            fakeRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<FormattableString>(), It.IsAny<object>()))
                .Returns(Task.FromResult<IEnumerable<FakeEntity>>(entitiesExpected));

            var entities = await fakeRepository.FindAsync($"Name = @name", new object());

            AssertExpectedObject(entitiesExpected, entities);
        }

        [Fact] 
        public void TestInvokeFindAll() 
        {
            var entitiesExpected = CreateTestFakeEntities();
            fakeRepositoryMock
                .Setup(r => r.FindAll())
                .Returns(entitiesExpected);

            var entities = fakeRepository.FindAll();

            AssertExpectedObject(entitiesExpected, entities);
        }

        [Fact] 
        public async void TestInvokeFindAllAsync() 
        {
            var entitiesExpected = CreateTestFakeEntities();
            fakeRepositoryMock
                .Setup(r => r.FindAllAsync())
                .Returns(Task.FromResult<IEnumerable<FakeEntity>>(entitiesExpected));

            var entities = await fakeRepository.FindAllAsync();
            
            AssertExpectedObject(entitiesExpected, entities);
        }
  
        [Fact] 
        public void TestInvokeFindOne() 
        {
            var entityExpected = CreateTestFakeEntity();
            fakeRepositoryMock
                .Setup(r => r.FindOne(entityExpected.Id))
                .Returns(entityExpected);

            var entity = fakeRepository.FindOne(entityExpected.Id);
            
            AssertExpectedObject(entityExpected, entity);
        }

        [Fact] 
        public async void TestInvokeFindOneAsync() 
        {
            var entityExpected = CreateTestFakeEntity();
            fakeRepositoryMock
                .Setup(r => r.FindOneAsync(entityExpected.Id))
                .Returns(Task.FromResult<FakeEntity>(entityExpected));

            var entity = await fakeRepository.FindOneAsync(entityExpected.Id);
            
            AssertExpectedObject(entityExpected, entity);
        }

        [Fact] 
        public void TestInvokeDelete() 
        {
            var entityExpected = CreateTestFakeEntity();

            fakeRepository.Delete(entityExpected.Id);
            fakeRepositoryMock.Verify(r => r.Delete(entityExpected.Id), Times.Once());
        }

        [Fact] 
        public async void TestInvokeDeleteAsync()
        {
            var entityExpected = CreateTestFakeEntity();

            await fakeRepository.DeleteAsync(entityExpected.Id);
            fakeRepositoryMock.Verify(r => r.DeleteAsync(entityExpected.Id), Times.Once());
        }

        [Fact]
        public void TestInvokeFindByNameIsNull()
        {
            var entityExpected = CreateTestFakeEntity();
            var entitiesExpected = new List<FakeEntity>() { entityExpected };
            fakeRepositoryMock
                .Setup(r => r.Find(It.IsAny<FormattableString>(), It.IsAny<object>()))
                .Returns(entitiesExpected);

            var entities = fakeRepository.FindByNameIsNull();

            AssertExpectedObject(entitiesExpected, entities);
        }

        [Fact]
        public void TestInvokeFindByName()
        {
            var entityExpected = CreateTestFakeEntity();
            var entitiesExpected = new List<FakeEntity>() { entityExpected };
            fakeRepositoryMock
                .Setup(r => r.Find(It.IsAny<FormattableString>(), It.IsAny<object>()))
                .Returns(entitiesExpected);

            var entities = fakeRepository.FindByName(entityExpected.Name);

            AssertExpectedObject(entitiesExpected, entities);
        }

        [Fact]
        public void TestInvokeFindByDateOfBirthBetween()
        {
            var entityExpected = CreateTestFakeEntity();
            var entitiesExpected = new List<FakeEntity>() { entityExpected };
            fakeRepositoryMock
                .Setup(r => r.Find(It.IsAny<FormattableString>(), It.IsAny<object>()))
                .Returns(entitiesExpected);

            var entities = fakeRepository.FindByDateOfBirthBetween(new DateTime(2000, 1, 1), new DateTime(2019, 1, 1));
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

        private FakeEntity CreateTestFakeEntity()
        {
            var id = faker.Random.Int(0, 100);
            var name = faker.Person.FullName;

            return new FakeEntity(id, name);
        }

        public interface IFakeRepository : ICrudRepository<FakeEntity, int>
        {
            IEnumerable<FakeEntity> FindByNameIsNull();

            IEnumerable<FakeEntity> FindByName(string name);
            
            IEnumerable<FakeEntity> FindByDateOfBirthBetween(DateTime dateOfBirthStart, DateTime dateOfBirthEnd);
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