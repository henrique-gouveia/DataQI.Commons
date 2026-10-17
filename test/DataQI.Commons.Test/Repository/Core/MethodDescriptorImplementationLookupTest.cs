using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Repository.Core;
using DataQI.Commons.Repository.Paging;
using DataQI.Commons.Test.Repository.Sample;

using Xunit;

namespace DataQI.Commons.Test.Repository.Core
{
    public class MethodDescriptorImplementationLookupTest
    {
        private static (string Name, Type[] ParameterTypes) Lookup(Type repository, string name, params Type[] parameterTypes)
        {
            var method = repository.GetMethods().Single(m =>
                m.Name == name && !m.IsGenericMethod && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes));

            (string, Type[]) requested = default;
            MethodDescriptor.Create(method, typeof(FakeEntity), new Dictionary<string, MethodInfo>(),
                (implementationName, implementationParameterTypes) =>
                {
                    requested = (implementationName, implementationParameterTypes);
                    return typeof(object).GetMethod(nameof(ToString));
                });
            return requested;
        }

        [Fact]
        public void TestSyncCollectionLooksForFind()
        {
            var requested = Lookup(typeof(IFakeRepository), "FindByFirstName", typeof(string));

            Assert.Equal("Find", requested.Name);
            Assert.Empty(requested.ParameterTypes);
        }

        [Fact]
        public void TestAsyncCollectionLooksForFindAsyncWithToken()
        {
            var requested = Lookup(typeof(IFakeRepository), "FindByFirstNameAsync", typeof(string));

            Assert.Equal("FindAsync", requested.Name);
            Assert.Equal(new[] { typeof(CancellationToken) }, requested.ParameterTypes);
        }

        [Fact]
        public void TestSyncSingleLooksForFindOne()
        {
            var requested = Lookup(typeof(IFakeRepository), "FindByEmail", typeof(string));

            Assert.Equal("FindOne", requested.Name);
            Assert.Empty(requested.ParameterTypes);
        }

        [Fact]
        public void TestAsyncSingleLooksForFindOneAsyncWithToken()
        {
            var requested = Lookup(typeof(IFakeRepository), "FindByEmailAsync", typeof(string));

            Assert.Equal("FindOneAsync", requested.Name);
            Assert.Equal(new[] { typeof(CancellationToken) }, requested.ParameterTypes);
        }

        [Fact]
        public void TestSyncSliceLooksForFindSliceWithPageRequest()
        {
            var requested = Lookup(typeof(IPagedFakeRepository), "FindByNameOrderByIdAsc", typeof(string), typeof(IPageRequest));

            Assert.Equal("FindSlice", requested.Name);
            Assert.Equal(new[] { typeof(IPageRequest) }, requested.ParameterTypes);
        }

        [Fact]
        public void TestAsyncSliceLooksForFindSliceAsyncWithPageRequestAndToken()
        {
            var requested = Lookup(typeof(IPagedFakeRepository), "FindByNameAsync", typeof(string), typeof(IPageRequest));

            Assert.Equal("FindSliceAsync", requested.Name);
            Assert.Equal(new[] { typeof(IPageRequest), typeof(CancellationToken) }, requested.ParameterTypes);
        }

        [Fact]
        public void TestSyncPageLooksForFindPageWithPageRequest()
        {
            var requested = Lookup(typeof(IPagedFakeRepository), "FindByName", typeof(string), typeof(IPageRequest));

            Assert.Equal("FindPage", requested.Name);
            Assert.Equal(new[] { typeof(IPageRequest) }, requested.ParameterTypes);
        }

        [Fact]
        public void TestAsyncPageLooksForFindPageAsyncWithPageRequestAndToken()
        {
            var requested = Lookup(typeof(IPagedFakeRepository), "FindByNameAsync", typeof(string), typeof(IPageRequest), typeof(CancellationToken));

            Assert.Equal("FindPageAsync", requested.Name);
            Assert.Equal(new[] { typeof(IPageRequest), typeof(CancellationToken) }, requested.ParameterTypes);
        }

        [Fact]
        public void TestExactMatchDoesNotLookForAnImplementation()
        {
            var method = typeof(IFakeRepository).GetMethods().First(m => m.Name == "FindByFirstName");
            var exact = new Dictionary<string, MethodInfo> { { method.UniqueName(), method } };
            var lookups = 0;

            MethodDescriptor.Create(method, typeof(FakeEntity), exact, (name, types) =>
            {
                lookups++;
                return null;
            });

            Assert.Equal(0, lookups);
        }
    }
}
