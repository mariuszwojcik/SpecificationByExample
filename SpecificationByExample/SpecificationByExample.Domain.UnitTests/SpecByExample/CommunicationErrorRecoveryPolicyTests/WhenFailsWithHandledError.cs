using System;
using System.Diagnostics;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;

namespace SpecificationByExample.Domain.UnitTests.SpecByExample.CommunicationErrorRecoveryPolicyTests
{
    [TestFixture]
    public class WhenFailsWithHandledError : CommunicationErrorRecoveryPolicyTestBase
    {
        [Test]
        public void ThenWaitsDefinedTimeBeforeRetrying()
        {
            var policy = new CommunicationErrorRecoveryPolicy<int>(4, TimeSpan.FromMilliseconds(500));

            TestService.Stub(i => i.Foo()).Throw(new WebException(TimedOutErrorMessage)).Repeat.Once();
            TestService.Stub(i => i.Foo()).Return(9);

            var watch = Stopwatch.StartNew();
            policy.Run(() => TestService.Foo());
            watch.Stop();

            Assert.That(watch.Elapsed, Is.EqualTo(policy.WaitBeforeRetry).Within(TimeSpan.FromMilliseconds(50)));
            TestService.VerifyAllExpectations();
        }

        [Test]
        public void SeveralTimesThenUsesExponentialBackofTime()
        {
            const int waitTime = 50;

            TestService.Stub(i => i.Foo()).Throw(new WebException(TimedOutErrorMessage)).Repeat.Times(3);
            TestService.Stub(i => i.Foo()).Return(9);

            var watch = Stopwatch.StartNew();
            Policy.Run(() => TestService.Foo());
            watch.Stop();

            const int expectedTime = waitTime + (2 * waitTime) + (4 * waitTime);
            Assert.That(watch.Elapsed, Is.EqualTo(TimeSpan.FromMilliseconds(expectedTime)).Within(TimeSpan.FromMilliseconds(waitTime)));
            TestService.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(WebException), ExpectedMessage = TimedOutErrorMessage)]
        public void MoreTimesThanAllowedThenFails()
        {
            TestService.Expect(i => i.Foo()).Throw(new WebException(TimedOutErrorMessage)).Repeat.Times(Policy.AllowedRetries + 1);

            Policy.Run(() => TestService.Foo());
        }
    }
}