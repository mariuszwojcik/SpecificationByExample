using System;
using System.Net;
using NUnit.Framework;
using Rhino.Mocks;
using SpecificationByExample.Domain.UnitTests.Dsl.extensions;

namespace SpecificationByExample.Domain.UnitTests.Dsl
{
    [TestFixture]
    public class WhenFailsWithHandledError : CommunicationErrorRecoveryPolicyTestBase
    {
        [Test]
        public void ThenWaitsDefinedTimeBeforeRetrying()
        {
            var policy = new CommunicationErrorRecoveryPolicy<int>(4, TimeSpan.FromMilliseconds(500));

            TestService.
                FailWithTimeOut().Once()
                .Then().Return(9);

            var elapsed = policy.RunAndMeasureExecutionTime(() => TestService.Foo());

            Assert.That(elapsed, Is.EqualTo(TimeSpan.FromMilliseconds(500)).Within(TimeSpan.FromMilliseconds(50)));
            TestService.VerifyAllExpectations();
        }

        [Test]
        public void SeveralTimesThenUsesExponentialBackofTime()
        {
            TestService
                .FailWithTimeOut().Times(3)
                .Then().Return(9);

            var elapsed = Policy.RunAndMeasureExecutionTime(() => TestService.Foo());

            var waitTime = Policy.WaitBeforeRetry.Milliseconds;
            var expectedTime = waitTime + (2*waitTime) + (4*waitTime);
            Assert.That(elapsed, Is.EqualTo(TimeSpan.FromMilliseconds(expectedTime)).Within(TimeSpan.FromMilliseconds(waitTime)));
            TestService.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(WebException), ExpectedMessage = TimedOutErrorMessage)]
        public void MoreTimesThanAllowedThenFails()
        {
            TestService
                .FailWithTimeOut().Times(Policy.AllowedRetries + 1)
                .Then().Return(9);

            Policy.Run(() => TestService.Foo());
        }
    }
}