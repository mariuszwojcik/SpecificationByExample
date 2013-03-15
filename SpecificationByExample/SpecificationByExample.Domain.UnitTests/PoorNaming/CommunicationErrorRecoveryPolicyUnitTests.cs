using System;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace SpecificationByExample.Domain.UnitTests.PoorNaming
{
    public class CommunicationErrorRecoveryPolicyUnitTests
    {
        private ITestService _testService;
        private CommunicationErrorRecoveryPolicy<int> _policy;

        [SetUp]
        public void TestSetUp()
        {
            _testService = MockRepository.GenerateMock<ITestService>();

            _policy = new CommunicationErrorRecoveryPolicy<int>(3, TimeSpan.FromMilliseconds(50));
        }

        [Test]
        public void Retry()
        {
            _policy = new CommunicationErrorRecoveryPolicy<int>(3, TimeSpan.FromMilliseconds(500));
            _testService.Expect(i => i.Foo()).Throw(new WebException("The operation has timed out")).Repeat.Once();
            _testService.Expect(i => i.Foo()).Return(9).Repeat.Once();

            var watch = Stopwatch.StartNew();
            _policy.Run(() => _testService.Foo());
            watch.Stop();

            Assert.That(watch.Elapsed, Is.EqualTo(TimeSpan.FromMilliseconds(500)).Within(TimeSpan.FromMilliseconds(50)));
            _testService.VerifyAllExpectations();
        }

        [Test]
        public void TimeOut_Success()
        {
            _testService.Expect(i => i.Foo()).Throw(new WebException("The operation has timed out")).Repeat.Once();
            _testService.Expect(i => i.Foo()).Return(9).Repeat.Once();

            var result = _policy.Run(() => _testService.Foo());

            Assert.AreEqual(9, result);
        }

        [Test]
        public void ConnectionDrop_Success()
        {
            _testService.Expect(i => i.Foo()).Throw(new WebException("The underlying connection was closed: A connection that was expected to be kept alive was closed by the server.")).Repeat.Once();
            _testService.Expect(i => i.Foo()).Return(9).Repeat.Once();

            var result = _policy.Run(() => _testService.Foo());

            Assert.AreEqual(9, result);
        }

        [Test]
        public void BadGateway_Success()
        {
            _testService.Expect(i => i.Foo()).Throw(new WebException("The request failed with HTTP status 502: Bad Gateway.")).Repeat.Once();
            _testService.Expect(i => i.Foo()).Return(9).Repeat.Once();

            var result = _policy.Run(() => _testService.Foo());

            Assert.AreEqual(9, result);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void OtherException_Failure()
        {
            _testService.Expect(i => i.Foo()).Throw(new Exception()).Repeat.Once();
            _testService.Expect(i => i.Foo()).Return(9).Repeat.Once();

            _policy.Run(() => _testService.Foo());
        }

        [Test]
        public void CommunicationException_Success()
        {
            _testService.Expect(i => i.Foo()).Throw(new CommunicationException("The request failed with HTTP status 502: Bad Gateway.")).Repeat.Once();
            _testService.Expect(i => i.Foo()).Return(9).Repeat.Once();

            var result = _policy.Run(() => _testService.Foo());

            Assert.AreEqual(9, result);
        }

        [Test]
        public void ExponentialBackofTime()
        {
            var waitTime = _policy.WaitBeforeRetry.Milliseconds;

            _testService.Expect(i => i.Foo()).Throw(new CommunicationException("The request failed with HTTP status 502: Bad Gateway.")).Repeat.Times(3);
            _testService.Expect(i => i.Foo()).Return(9).Repeat.Once();

            var watch = Stopwatch.StartNew();
            _policy.Run(() => _testService.Foo());
            watch.Stop();

            var expectedTime = waitTime + (2 * waitTime) + (4 * waitTime);
            Assert.That(watch.Elapsed, Is.EqualTo(TimeSpan.FromMilliseconds(expectedTime)).Within(TimeSpan.FromMilliseconds(waitTime)));
            _testService.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(WebException))]
        public void AllowedRetries()
        {
            _policy = new CommunicationErrorRecoveryPolicy<int>(3, TimeSpan.FromMilliseconds(50));
            _testService.Expect(i => i.Foo()).Throw(new WebException("The request failed with HTTP status 502: Bad Gateway.")).Repeat.Times(4);

            _policy.Run(() => _testService.Foo());
        }

        [TestCase(0, 50)]
        [TestCase(11, 50)]
        [TestCase(3, 49)]
        [TestCase(3, 60001)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AllowedRetries_InvalidValue_Failure(int retries, int wait)
        {
            new CommunicationErrorRecoveryPolicy<int>(retries, TimeSpan.FromMilliseconds(wait));
        }
    }
}