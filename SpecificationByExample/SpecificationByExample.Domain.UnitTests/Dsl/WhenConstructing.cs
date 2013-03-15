using System;
using NUnit.Framework;
using SpecificationByExample.Domain.UnitTests.Dsl.extensions;

namespace SpecificationByExample.Domain.UnitTests.Dsl
{
    [TestFixture]
    public class WhenConstructing : CommunicationErrorRecoveryPolicyTestBase
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
        [ExpectedArgumentOutOfRangeException("waitBeforeRetry", "Value must be between 50 milliseconds and 1 minute.")]
        public void WithWaitBeforeRetryLessThan50MillisecondsThenFails()
        {
            new CommunicationErrorRecoveryPolicy<int>(5, TimeSpan.FromMilliseconds(49));
        }

        [Test]
        [ExpectedArgumentOutOfRangeException("waitBeforeRetry", "Value must be between 50 milliseconds and 1 minute.")]
        public void WithWaitBeforeRetryGreaterThan1MinuteThenFails()
        {
            new CommunicationErrorRecoveryPolicy<int>(5, TimeSpan.FromMilliseconds(49));
        }

        [Test]
        [ExpectedArgumentOutOfRangeException("allowedRetries", "Value must be between 1 and 10.")]
        public void WithAllowedRetriesLessThan1ThenFails()
        {
            new CommunicationErrorRecoveryPolicy<int>(0, TimeSpan.FromSeconds(5));
        }

        [Test]
        [ExpectedArgumentOutOfRangeException("allowedRetries", "Value must be between 1 and 10.")]
        public void WithAllowedRetriesGreaterThan10ThenFails()
        {
            new CommunicationErrorRecoveryPolicy<int>(11, TimeSpan.FromSeconds(5));
        }
    }
}