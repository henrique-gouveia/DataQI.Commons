using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Xunit;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Repository.Core;
using DataQI.Commons.Repository.Paging;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Core
{
    public class MethodDescriptorPagingTest
    {
        private static MethodInfo Repo(string name)
            => typeof(IPagedRepository<FakeEntity, int>).GetMethods().Single(m => m.Name == name);

        private static MethodInfo Query(string name, int parameterCount, string returnContains)
            => typeof(IPagedFakeRepository).GetMethods().Single(m =>
                m.Name == name && m.GetParameters().Length == parameterCount &&
                m.ReturnType.ToString().Contains(returnContains));

        private static PagedMethods AllPaged() => new PagedMethods(
            Repo("FindSlice"), Repo("FindSliceAsync"), Repo("FindPage"), Repo("FindPageAsync"));

        private static MethodDescriptor Describe(MethodInfo method, PagedMethods paged = null)
            => MethodDescriptor.Create(method, typeof(FakeEntity), new Dictionary<string, MethodInfo>(),
                null, null, null, null, paged ?? AllPaged());

        [Fact]
        public void TestClassifiesSyncSlice()
        {
            var descriptor = Describe(Query("FindByNameOrderByIdAsc", 2, "ISlice"));

            Assert.Equal(DispatchKind.SyncSlice, descriptor.Kind);
            Assert.Equal("FindSlice", descriptor.ResolvedMethod.Name);
            Assert.Equal(1, descriptor.PageRequestIndex);
            Assert.False(descriptor.HasCancellationToken);
        }

        [Fact]
        public void TestClassifiesSyncPage()
        {
            var descriptor = Describe(Query("FindByName", 2, "IPage"));

            Assert.Equal(DispatchKind.SyncPage, descriptor.Kind);
            Assert.Equal("FindPage", descriptor.ResolvedMethod.Name);
        }

        [Fact]
        public void TestClassifiesAsyncSliceBeforeTheEntityCheck()
        {
            var descriptor = Describe(Query("FindByNameAsync", 2, "ISlice"));

            Assert.Equal(DispatchKind.AsyncSlice, descriptor.Kind);
            Assert.Equal("FindSliceAsync", descriptor.ResolvedMethod.Name);
            Assert.Equal(typeof(ISlice<FakeEntity>), descriptor.ResultType);
        }

        [Fact]
        public void TestClassifiesAsyncPageWithCancellationToken()
        {
            var descriptor = Describe(Query("FindByNameAsync", 3, "IPage"));

            Assert.Equal(DispatchKind.AsyncPage, descriptor.Kind);
            Assert.True(descriptor.HasCancellationToken);
            Assert.Equal(1, descriptor.PageRequestIndex);
        }

        [Fact]
        public void TestPagingContractMethodsStayExactMatch()
        {
            var findSlice = Repo("FindSlice");
            var exact = new Dictionary<string, MethodInfo> { [findSlice.UniqueName()] = findSlice };

            var descriptor = MethodDescriptor.Create(findSlice, typeof(FakeEntity), exact,
                null, null, null, null, AllPaged());

            Assert.Equal(DispatchKind.ExactMatch, descriptor.Kind);
            Assert.Equal(-1, descriptor.PageRequestIndex);
        }

        [Fact]
        public void TestMethodsWithoutPagingKeepNegativePageRequestIndex()
        {
            var findByFirstName = typeof(IFakeRepository).GetMethods().First(m => m.Name == "FindByFirstName");
            var find = typeof(IFakeRepository).GetAllInterfaceMethods()
                .First(m => m.Name == "Find" && m.GetParameters().Length == 1);

            var descriptor = MethodDescriptor.Create(findByFirstName, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), find, null, null, null);

            Assert.Equal(DispatchKind.SyncCollection, descriptor.Kind);
            Assert.Equal(-1, descriptor.PageRequestIndex);
        }

        [Fact]
        public void TestNewDispatchKindsAreAppendedAfterUnresolvable()
        {
            Assert.True((int)DispatchKind.SyncSlice > (int)DispatchKind.Unresolvable);
            Assert.Equal(5, (int)DispatchKind.Unresolvable);
        }
    }
}
