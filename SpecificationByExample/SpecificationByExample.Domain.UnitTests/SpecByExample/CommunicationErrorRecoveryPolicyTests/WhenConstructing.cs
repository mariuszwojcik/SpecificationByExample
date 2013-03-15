using System;
using NUnit.Framework;

namespace SpecificationByExample.Domain.UnitTests.SpecByExample.CommunicationErrorRecoveryPolicyTests
{
    [TestFixture]
    public class WhenConstructing
    {
        [Test]
        public void ThenInitialisesWaitBeforeRetryTo15Seconds()
        {
            var policy = new CommunicationErrorRecoveryPolicy<int>();

            Assert.That(policy.WaitBeforeRetry, Is.EqualTo(TimeSpan.FromSeconds(15)));
        }

        [Test]
        public void ThenInitialisesAllowedRetiresTo5()
        {
            var policy = new CommunicationErrorRecoveryPolicy<int>();

            Assert.That(policy.AllowedRetries, Is.EqualTo(5));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), ExpectedMessage = "Value must be between 50 milliseconds and 1 minute.\r\nParameter name: waitBeforeRetry")]
        public void WithWaitBeforeRetryLessThan50MillisecondsThenFails()
        {
            new CommunicationErrorRecoveryPolicy<int>(5, TimeSpan.FromMilliseconds(49));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), ExpectedMessage = "Value must be between 50 milliseconds and 1 minute.\r\nParameter name: waitBeforeRetry")]
        public void WithWaitBeforeRetryGreaterThan1MinuteThenFails()
        {
            new CommunicationErrorRecoveryPolicy<int>(5, TimeSpan.FromMilliseconds(49));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), ExpectedMessage = "Value must be between 1 and 10.\r\nParameter name: allowedRetries")]
        public void WithAllowedRetriesLessThan1ThenFails()
        {
            new CommunicationErrorRecoveryPolicy<int>(0, TimeSpan.FromSeconds(5));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), ExpectedMessage = "Value must be between 1 and 10.\r\nParameter name: allowedRetries")]
        public void WithAllowedRetriesGreaterThan10ThenFails()
        {
            new CommunicationErrorRecoveryPolicy<int>(11, TimeSpan.FromSeconds(5));
        }
    }
}