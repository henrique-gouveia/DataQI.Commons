using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using Xunit;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Repository.Core;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Core
{
    public class MethodDescriptorTest
    {
        private static readonly IReadOnlyDictionary<string, MethodInfo> RepositoryMethods =
            typeof(IEntityRepository<FakeEntity>).GetAllInterfaceMethods()
                .ToDictionary(m => m.UniqueName(), m => m);

        private static readonly IReadOnlyDictionary<string, MethodInfo> FakeRepositoryMethods =
            typeof(IFakeRepository).GetAllInterfaceMethods()
                .ToDictionary(m => m.UniqueName(), m => m);

        private static MethodInfo Method(string uniqueNameContains, IReadOnlyDictionary<string, MethodInfo> source)
            => source.Values.First(m => m.UniqueName().Contains(uniqueNameContains));

        [Fact]
        public void TestCreateClassifiesExactMatchMethod()
        {
            var insertMethod = Method("Insert:", RepositoryMethods);
            var exactMatchMethods = new Dictionary<string, MethodInfo> { [insertMethod.UniqueName()] = insertMethod };

            var descriptor = MethodDescriptor.Create(insertMethod, typeof(FakeEntity), exactMatchMethods,
                findMethod: null, findAsyncMethod: null, findOneMethod: null, findOneAsyncMethod: null);

            Assert.Equal(DispatchKind.ExactMatch, descriptor.Kind);
            Assert.Same(insertMethod, descriptor.ResolvedMethod);
            Assert.Equal(insertMethod.UniqueName(), descriptor.UniqueName);
        }

        [Fact]
        public void TestCreateClassifiesSyncCollectionMethod()
        {
            var findByFirstNameMethod = Method("FindByFirstName:", FakeRepositoryMethods);
            var findMethod = RepositoryMethods.Values.First(m =>
                m.Name == "Find" && m.GetParameters().Length == 1);

            var descriptor = MethodDescriptor.Create(findByFirstNameMethod, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), findMethod, findAsyncMethod: null, findOneMethod: null, findOneAsyncMethod: null);

            Assert.Equal(DispatchKind.SyncCollection, descriptor.Kind);
            Assert.Same(findMethod, descriptor.ResolvedMethod);
            Assert.False(descriptor.HasCancellationToken);
        }

        [Fact]
        public void TestCreateClassifiesAsyncCollectionMethod()
        {
            var findByFirstNameAsyncMethod = FakeRepositoryMethods.Values.First(m =>
                m.Name == nameof(IFakeRepository.FindByFirstNameAsync) && m.GetParameters().Length == 1);
            var findAsyncMethod = RepositoryMethods.Values.First(m => m.Name == "FindAsync");

            var descriptor = MethodDescriptor.Create(findByFirstNameAsyncMethod, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), findMethod: null, findAsyncMethod, findOneMethod: null, findOneAsyncMethod: null);

            Assert.Equal(DispatchKind.AsyncCollection, descriptor.Kind);
            Assert.Same(findAsyncMethod, descriptor.ResolvedMethod);
            Assert.False(descriptor.HasCancellationToken);
        }

        [Fact]
        public void TestCreateClassifiesAsyncCollectionMethodWithCancellationToken()
        {
            var findByFirstNameAsyncWithTokenMethod = FakeRepositoryMethods.Values.First(m =>
                m.Name == nameof(IFakeRepository.FindByFirstNameAsync) && m.GetParameters().Length == 2);
            var findAsyncMethod = RepositoryMethods.Values.First(m => m.Name == "FindAsync");

            var descriptor = MethodDescriptor.Create(findByFirstNameAsyncWithTokenMethod, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), findMethod: null, findAsyncMethod, findOneMethod: null, findOneAsyncMethod: null);

            Assert.Equal(DispatchKind.AsyncCollection, descriptor.Kind);
            Assert.True(descriptor.HasCancellationToken);
        }

        [Fact]
        public void TestCreateClassifiesSyncSingleMethod()
        {
            var findByEmailMethod = Method("FindByEmail:", FakeRepositoryMethods);
            var findOneMethod = RepositoryMethods.Values.First(m =>
                m.Name == "FindOne" && m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType.IsGenericType);

            var descriptor = MethodDescriptor.Create(findByEmailMethod, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), findMethod: null, findAsyncMethod: null, findOneMethod, findOneAsyncMethod: null);

            Assert.Equal(DispatchKind.SyncSingle, descriptor.Kind);
            Assert.Same(findOneMethod, descriptor.ResolvedMethod);
        }

        [Fact]
        public void TestCreateClassifiesAsyncSingleMethod()
        {
            var findByEmailAsyncMethod = FakeRepositoryMethods.Values.First(m =>
                m.Name == nameof(IFakeRepository.FindByEmailAsync) && m.GetParameters().Length == 1);
            var findOneAsyncMethod = RepositoryMethods.Values.First(m =>
                m.Name == "FindOneAsync" && m.GetParameters()[0].ParameterType.IsGenericType);

            var descriptor = MethodDescriptor.Create(findByEmailAsyncMethod, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), findMethod: null, findAsyncMethod: null, findOneMethod: null, findOneAsyncMethod);

            Assert.Equal(DispatchKind.AsyncSingle, descriptor.Kind);
            Assert.Same(findOneAsyncMethod, descriptor.ResolvedMethod);
        }

        [Fact]
        public void TestCreateClassifiesUnresolvableSyncMethod()
        {
            var notImplementedMethod = Method("NotImplementedMethod:", FakeRepositoryMethods);

            var descriptor = MethodDescriptor.Create(notImplementedMethod, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), findMethod: null, findAsyncMethod: null, findOneMethod: null, findOneAsyncMethod: null);

            Assert.Equal(DispatchKind.Unresolvable, descriptor.Kind);
            Assert.Null(descriptor.ResolvedMethod);
        }

        [Fact]
        public void TestCreateClassifiesUnresolvableAsyncMethod()
        {
            var findByFirstNameAsyncMethod = FakeRepositoryMethods.Values.First(m =>
                m.Name == nameof(IFakeRepository.FindByFirstNameAsync) && m.GetParameters().Length == 1);

            var descriptor = MethodDescriptor.Create(findByFirstNameAsyncMethod, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), findMethod: null, findAsyncMethod: null, findOneMethod: null, findOneAsyncMethod: null);

            Assert.Equal(DispatchKind.Unresolvable, descriptor.Kind);
            Assert.Null(descriptor.ResolvedMethod);
        }

        [Fact]
        public void TestCreateDisambiguatesFindOneIdOverloadFromCriteriaOverload()
        {
            var findOneByIdMethod = RepositoryMethods.Values.First(m =>
                m.Name == "FindOne" && m.GetParameters().Length == 1 &&
                !m.GetParameters()[0].ParameterType.IsGenericType);
            var findOneByCriteriaMethod = RepositoryMethods.Values.First(m =>
                m.Name == "FindOne" && m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType.IsGenericType);

            // FindOne(TId) is an exact-match method (it's on the standard CRUD contract, invoked pass-through)
            var exactMatchMethods = new Dictionary<string, MethodInfo> { [findOneByIdMethod.UniqueName()] = findOneByIdMethod };

            var descriptor = MethodDescriptor.Create(findOneByIdMethod, typeof(FakeEntity), exactMatchMethods,
                findMethod: null, findAsyncMethod: null, findOneMethod: findOneByCriteriaMethod, findOneAsyncMethod: null);

            Assert.Equal(DispatchKind.ExactMatch, descriptor.Kind);
            Assert.Same(findOneByIdMethod, descriptor.ResolvedMethod);
        }

        [Fact]
        public void TestCreateExposesUnwrappedAsyncResultType()
        {
            var findByFirstNameAsyncMethod = FakeRepositoryMethods.Values.First(m =>
                m.Name == nameof(IFakeRepository.FindByFirstNameAsync) && m.GetParameters().Length == 1);
            var findAsyncMethod = RepositoryMethods.Values.First(m => m.Name == "FindAsync");

            var descriptor = MethodDescriptor.Create(findByFirstNameAsyncMethod, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), findMethod: null, findAsyncMethod, findOneMethod: null, findOneAsyncMethod: null);

            Assert.Equal(DispatchKind.AsyncCollection, descriptor.Kind);
            Assert.Equal(typeof(IEnumerable<FakeEntity>), descriptor.ResultType);
        }

        [Fact]
        public void TestCreateClassifiesAsyncSingleMethodWithCancellationToken()
        {
            var findByEmailAsyncMethod = FakeRepositoryMethods.Values.First(m =>
                m.Name == nameof(IFakeRepository.FindByEmailAsync) && m.GetParameters().Length == 2);
            var findOneAsyncMethod = RepositoryMethods.Values.First(m =>
                m.Name == "FindOneAsync" && m.GetParameters()[0].ParameterType.IsGenericType);

            var descriptor = MethodDescriptor.Create(findByEmailAsyncMethod, typeof(FakeEntity),
                new Dictionary<string, MethodInfo>(), findMethod: null, findAsyncMethod: null, findOneMethod: null, findOneAsyncMethod);

            Assert.Equal(DispatchKind.AsyncSingle, descriptor.Kind);
            Assert.Equal(typeof(FakeEntity), descriptor.ResultType);
            Assert.True(descriptor.HasCancellationToken);
        }
    }
}
