using System;
using System.Reflection;
using Moq;
using Net.Data.Commons.Repository;
using Net.Data.Commons.Repository.Core;
using Xunit;

namespace Net.Data.Commons.Test.Repository.Core
{
    public class RepositoryProxyTest
    {
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
            var fakeRepositoryMock = new Mock<ICrudRepository<object, int>>();
            var fakeRepository = RepositoryProxy.Create<ICrudRepository<Object, int>>(() => fakeRepositoryMock.Object);

            fakeRepository.Insert(new Object());
            fakeRepositoryMock.Verify(r => r.Insert(It.IsAny<Object>()), Times.Once());
        }
    }
}