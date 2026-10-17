using System;
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

        private static Func<string, Type[], MethodInfo> AllPaged() => FindImplementations.Of(("FindSlice", Repo("FindSlice")), ("FindSliceAsync", Repo("FindSliceAsync")), ("FindPage", Repo("FindPage")), ("FindPageAsync", Repo("FindPageAsync")));

        private static MethodDescriptor Describe(MethodInfo method, Func<string, Type[], MethodInfo> paged = null)
            => MethodDescriptor.Create(method, typeof(FakeEntity), new Dictionary<string, MethodInfo>(),
                paged ?? AllPaged());

        [Fact]
        public void TestClassifiesSyncSlice()
        {
            var descriptor = Describe(Query("FindByNameOrderByIdAsc", 2, "ISlice"));

            Assert.Equal(DispatchKind.SyncSlice, descriptor.Kind);
            Assert.Equal("FindSlice", descriptor.ResolvedMethod.Name);
            Assert.True(descriptor.HasPageRequest);
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
            Assert.True(descriptor.HasPageRequest);
            Assert.True(descriptor.HasCancellationToken);
        }

        [Fact]
        public void TestPagingContractMethodsStayExactMatch()
        {
            var findSlice = Repo("FindSlice");
            var exact = new Dictionary<string, MethodInfo> { [findSlice.UniqueName()] = findSlice };

            var descriptor = MethodDescriptor.Create(findSlice, typeof(FakeEntity), exact,
                AllPaged());

            Assert.Equal(DispatchKind.ExactMatch, descriptor.Kind);
            Assert.False(descriptor.HasPageRequest);
        }

        [Fact]
        public void TestMethodsWithoutPagingHaveNoPageRequest()
        {
            var findByFirstName = typeof(IFakeRepository).GetMethods().First(m => m.Name == "FindByFirstName");
            var find = typeof(IFakeRepository).GetAllInterfaceMethods()
                .First(m => m.Name == "Find" && m.GetParameters().Length == 1);

            var descriptor = MethodDescriptor.Create(findByFirstName, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), FindImplementations.Of(("Find", find)));

            Assert.Equal(DispatchKind.SyncCollection, descriptor.Kind);
            Assert.False(descriptor.HasPageRequest);
        }
    }
}
