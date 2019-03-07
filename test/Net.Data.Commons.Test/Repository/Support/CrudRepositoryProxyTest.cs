using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Moq;

using Net.Data.Commons.Repository;
using Net.Data.Commons.Repository.Support;

using Xunit;

namespace Net.Data.Commons.Test.Repository.Support
{
    public class CrudRepositoryProxyTest
    {
        [Fact]
        public void TestRejectsNullRepository()
        {
            var exception = Assert.Throws<TargetInvocationException>(() => 
                new CrudRepositoryProxyFactory<Object, int, FakeRepositoryProxyWithReturnsNullCrudRepository>()
                    .Create<ICrudRepository<Object, int>>());
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Repository must not be null", exceptionMessage);
        }

        // [Fact]
        // public void TestRejectsAnyInvokeWhenNotCreatedByAnyRepositoryProxyFactory() 
        // {
        //     var exception = Assert.Throws<InvalidBuildingMethodException>(() =>
        //     {
        //         var repository = FakeRepositoryProxy.Create<ICrudRepository<Object, int>, FakeRepositoryProxy>();
        //         repository.Save(new Object());
        //     });
        //     var exceptionMessage = exception.GetBaseException().Message;

        //     Assert.Equal("Any RepositoryProxy must be created by a CrudRepositoryProxyFactory", exceptionMessage);
        // }

        [Fact] 
        public void TestInvokeInsert() 
        {
            var fakeRepository = new FakeCrudRepositoryProxyFactory().Create<ICrudRepository<Object, int>>();
            var fakeRepositoryMock = ((FakeRepositoryProxy) fakeRepository).CrudRepositoryMock;

            fakeRepository.Insert(new Object());
            fakeRepositoryMock.Verify(r => r.Insert(It.IsAny<Object>()), Times.Once());
        }

        public class FakeRepository : Mock<ICrudRepository<Object, int>>
        {
        }

        private class FakeCrudRepositoryProxyFactory : CrudRepositoryProxyFactory<Object, int, FakeRepositoryProxy>
        {
        }

        public class FakeRepositoryProxy : CrudRepositoryProxy<Object, int>
        {
            protected override ICrudRepository<Object, int> CreateRepository() 
            {
                CrudRepositoryMock = new Mock<ICrudRepository<object, int>>();
                return CrudRepositoryMock.Object;
            }
            
            public Mock<ICrudRepository<Object, int>> CrudRepositoryMock { get; private set; }
        }

        public class FakeRepositoryProxyWithReturnsNullCrudRepository : CrudRepositoryProxy<Object, int>
        {
            protected override ICrudRepository<Object, int> CreateRepository() => null;
        }
    }
}