using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Moq;
using Xunit;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Repository.Core;
using DataQI.Commons.Repository.Paging;
using DataQI.Commons.Repository.Paging.Support;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Repository.Core
{
    public class RepositoryProxyPagingTest
    {
        private readonly Mock<IPagedRepository<FakeEntity, int>> defaultMock = new Mock<IPagedRepository<FakeEntity, int>>();
        private readonly IPagedFakeRepository repository;

        public RepositoryProxyPagingTest()
        {
            repository = RepositoryProxy<IPagedFakeRepository>.Create(() => defaultMock.Object);
        }

        private static ICriteria Apply(Func<ICriteria, ICriteria> builder) => builder(new Criteria());

        [Fact]
        public void TestSyncSliceReceivesTheSamePageRequestAndTheQueryPlanCriteria()
        {
            var page = PageRequest.Of(1, 5);
            Func<ICriteria, ICriteria> captured = null;
            var expected = new Slice<FakeEntity>([], 1, 5, false);
            defaultMock.Setup(r => r.FindSlice(It.IsAny<Func<ICriteria, ICriteria>>(), page))
                .Callback<Func<ICriteria, ICriteria>, IPageRequest>((b, _) => captured = b)
                .Returns(expected);

            var result = repository.FindByNameOrderByIdAsc("ana", page);

            Assert.Same(expected, result);
            var criteria = Apply(captured);
            Assert.Single(criteria.Criterions);
            Assert.Equal("Id", criteria.Orders.Single().PropertyName);
        }

        [Fact]
        public void TestSyncPageIsForwardedToFindPage()
        {
            var page = PageRequest.Of(0, 10);
            var expected = new Page<FakeEntity>([], 0, 10, 0);
            defaultMock.Setup(r => r.FindPage(It.IsAny<Func<ICriteria, ICriteria>>(), page)).Returns(expected);

            Assert.Same(expected, repository.FindByName("ana", page));
        }

        [Fact]
        public async Task TestAsyncSliceWithoutTokenForwardsDefaultToken()
        {
            var page = PageRequest.Of(0, 10);
            var expected = new Slice<FakeEntity>([], 0, 10, false);
            defaultMock.Setup(r => r.FindSliceAsync(It.IsAny<Func<ICriteria, ICriteria>>(), page, default(CancellationToken)))
                .ReturnsAsync(expected);

            Assert.Same(expected, await repository.FindByNameAsync("ana", page));
        }

        [Fact]
        public async Task TestAsyncPageForwardsTheCallersToken()
        {
            var page = PageRequest.Of(2, 10);
            using (var source = new CancellationTokenSource())
            {
                var expected = new Page<FakeEntity>([], 2, 10, 5);
                defaultMock.Setup(r => r.FindPageAsync(It.IsAny<Func<ICriteria, ICriteria>>(), page, source.Token))
                    .ReturnsAsync(expected);

                Assert.Same(expected, await repository.FindByNameAsync("ana", page, source.Token));
            }
        }

        [Fact]
        public void TestPageRequestIsStrippedBeforeTheQueryPlan()
        {
            var page = PageRequest.Of(0, 10);
            defaultMock.Setup(r => r.FindPage(It.IsAny<Func<ICriteria, ICriteria>>(), page))
                .Returns(new Page<FakeEntity>([], 0, 10, 0));
            var recording = RecordingProxy.Create(() => defaultMock.Object);

            recording.FindByName("ana", page);

            Assert.Equal(["ana"], Assert.Single(RecordingProxy.ReceivedBy(recording)));
        }

        [Fact]
        public async Task TestPageRequestAndTokenAreStrippedBeforeTheQueryPlan()
        {
            var page = PageRequest.Of(0, 10);
            defaultMock.Setup(r => r.FindPageAsync(It.IsAny<Func<ICriteria, ICriteria>>(), page, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Page<FakeEntity>([], 0, 10, 0));
            var recording = RecordingProxy.Create(() => defaultMock.Object);

            await recording.FindByNameAsync("ana", page, CancellationToken.None);

            Assert.Equal(["ana"], Assert.Single(RecordingProxy.ReceivedBy(recording)));
        }

        [Fact]
        public void TestPageRequestIsStrippedForOrderedSliceMethods()
        {
            var page = PageRequest.Of(0, 10);
            defaultMock.Setup(r => r.FindSlice(It.IsAny<Func<ICriteria, ICriteria>>(), page))
                .Returns(new Slice<FakeEntity>([], 0, 10, false));
            var recording = RecordingProxy.Create(() => defaultMock.Object);

            recording.FindByNameOrderByIdAsc("ana", page);

            Assert.Equal(["ana"], Assert.Single(RecordingProxy.ReceivedBy(recording)));
        }

        [Fact]
        public void TestSyncSliceStripsTheOptionalTrailingToken()
        {
            var page = PageRequest.Of(0, 10);
            defaultMock.Setup(r => r.FindSlice(It.IsAny<Func<ICriteria, ICriteria>>(), page))
                .Returns(new Slice<FakeEntity>([], 0, 10, false));
            var recording = RecordingProxy.Create(() => defaultMock.Object);

            recording.FindByNameOrderByIdAsc("ana", page, CancellationToken.None);

            Assert.Equal(["ana"], Assert.Single(RecordingProxy.ReceivedBy(recording)));
        }

        [Fact]
        public void TestNullPageRequestReachesTheRepositoryUnchanged()
        {
            defaultMock.Setup(r => r.FindSlice(It.IsAny<Func<ICriteria, ICriteria>>(), null))
                .Throws(new ArgumentException("Page must not be null"));

            var exception = Assert.Throws<TargetInvocationException>(() =>
                repository.FindByNameOrderByIdAsc("ana", null));

            Assert.IsType<ArgumentException>(exception.GetBaseException());
        }

        [Fact]
        public void TestDirectFindSliceCallStaysExactMatch()
        {
            var page = PageRequest.Of(0, 3);
            Func<ICriteria, ICriteria> builder = c => c;
            var expected = new Slice<FakeEntity>([], 0, 3, false);
            defaultMock.Setup(r => r.FindSlice(builder, page)).Returns(expected);

            Assert.Same(expected, repository.FindSlice(builder, page));
        }

        [Fact]
        public void TestProxyCreationFailsWhenTheProviderDoesNotImplementPagedRepository()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                RepositoryProxy<IPagedFakeRepository>.Create(() => new CustomFakeRepository()));

            Assert.IsType<InvalidOperationException>(exception.GetBaseException());
            Assert.Contains("IPagedRepository", exception.GetBaseException().Message);
        }

        public class RecordingProxy : RepositoryProxy<IPagedFakeRepository>
        {
            private readonly List<object[]> received = [];

            public new static IPagedFakeRepository Create(Func<object> defaultRepositoryFactory)
            {
                DefaultRepositoryFactory = defaultRepositoryFactory;
                return DispatchProxy.Create<IPagedFakeRepository, RecordingProxy>();
            }

            public static IReadOnlyList<object[]> ReceivedBy(IPagedFakeRepository repository)
                => ((RecordingProxy)(object)repository).received;

            protected override Func<ICriteria, ICriteria> CreateCriteriaBuilder(MethodInfo targetMethod, object[] args)
            {
                received.Add(args);
                return base.CreateCriteriaBuilder(targetMethod, args);
            }
        }
    }
}
