using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Xunit;

using DataQI.Commons.Repository.Core;
using DataQI.Commons.Repository.Paging;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Core
{
    public class MethodDescriptorPagingSignatureTest
    {
        private static MethodInfo Repo(string name)
            => typeof(IPagedRepository<FakeEntity, int>).GetMethods().Single(m => m.Name == name);

        private static FindMethods AllPaged() => new FindMethods(
            null, null, null, null, Repo("FindSlice"), Repo("FindSliceAsync"), Repo("FindPage"), Repo("FindPageAsync"));

        private static MethodDescriptor Describe(System.Type repository, string name, FindMethods paged)
        {
            var method = repository.GetMethods().Single(m => m.Name == name);
            return MethodDescriptor.Create(method, typeof(FakeEntity), new Dictionary<string, MethodInfo>(),
                paged);
        }

        [Theory]
        [InlineData(typeof(IPagedWithoutRequestRepository), "FindByName", "IPageRequest")]
        [InlineData(typeof(IRequestWithPlainReturnRepository), "FindByName", "IPageRequest")]
        [InlineData(typeof(IDuplicatedRequestRepository), "FindByName", "one IPageRequest")]
        [InlineData(typeof(INonTrailingTokenPagedRepository), "FindByNameAsync", "CancellationToken")]
        [InlineData(typeof(IDuplicatedTokenPagedRepository), "FindByNameAsync", "CancellationToken")]
        [InlineData(typeof(IRequestNotLastRepository), "FindByName", "last")]
        public void TestRejectsInvalidSignatures(System.Type repository, string name, string messagePart)
        {
            var exception = Assert.Throws<InvalidOperationException>(() => Describe(repository, name, AllPaged()));

            Assert.Contains(repository.Name, exception.Message);
            Assert.Contains(name, exception.Message);
            Assert.Contains(messagePart, exception.Message);
        }

        [Theory]
        [InlineData(typeof(IWrongEntityPagedRepository))]
        [InlineData(typeof(IWrongEntityPageRepository))]
        [InlineData(typeof(IWrongEntityAsyncSliceRepository))]
        [InlineData(typeof(IWrongEntityAsyncPageRepository))]
        public void TestPagedResultMustUseTheRepositoryEntity(Type repository)
        {
            var exception = Assert.Throws<InvalidOperationException>(() => Describe(repository, "FindByName", AllPaged()));

            Assert.Contains(repository.Name, exception.Message);
            Assert.Contains("repository entity", exception.Message);
            Assert.Contains(typeof(FakeEntity).FullName, exception.Message);
            Assert.Contains(typeof(string).FullName, exception.Message);
        }

        [Fact]
        public void TestAcceptsRequestBeforeTrailingCancellationToken()
        {
            var descriptor = Describe(typeof(IRequestBeforeTokenRepository), "FindByNameAsync", AllPaged());

            Assert.Equal(DispatchKind.AsyncSlice, descriptor.Kind);
        }

        [Fact]
        public void TestRejectsPagedMethodWhenProviderDoesNotImplementPagedRepository()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Describe(typeof(IPagedFakeRepository), "FindByName", FindMethods.None));

            Assert.Contains("IPagedRepository", exception.Message);
        }
    }
}
