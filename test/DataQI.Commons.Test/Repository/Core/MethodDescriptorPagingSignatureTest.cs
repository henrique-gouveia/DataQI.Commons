using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
        private static PagedMethods AllPaged() => new PagedMethods(
        Repo("FindSlice"), Repo("FindSliceAsync"), Repo("FindPage"), Repo("FindPageAsync"));
        private static MethodDescriptor Describe(System.Type repository, string name, PagedMethods paged)
        {
            var method = repository.GetMethods().Single(m => m.Name == name);
            return MethodDescriptor.Create(method, typeof(FakeEntity), new Dictionary<string, MethodInfo>(),
            null, null, null, null, paged);
        }

        [Theory]
        [InlineData(typeof(IPagedWithoutRequestRepository), "FindByName", "IPageRequest")]
        [InlineData(typeof(IRequestWithPlainReturnRepository), "FindByName", "IPageRequest")]
        [InlineData(typeof(IDuplicatedRequestRepository), "FindByName", "one IPageRequest")]
        [InlineData(typeof(IWrongEntityPagedRepository), "FindByName", "IPageRequest")]
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
            Describe(typeof(IPagedFakeRepository), "FindByName", PagedMethods.None));

            Assert.Contains("IPagedRepository", exception.Message);
        }

    }
}
