using System;
using NUnit.Framework;
using Rhino.Mocks;
using SpecificationByExample.Domain.UnitTests.Dsl.extensions;

namespace SpecificationByExample.Domain.UnitTests.Dsl
{
    [TestFixture]
    public class WhenOperationFailsWithUnsupportedException : CommunicationErrorRecoveryPolicyTestBase
    {
        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = TimedOutErrorMessage)]
        public void BecauseOfTimeoutThenRetiresFails()
        {
            TestService
                .FailWithUnhandledException(TimedOutErrorMessage).Once()
                .Then().Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = ConnectionDropErrorMessage)]
        public void BecauseOfConnectionDropsThenFails()
        {
            TestService
                .FailWithUnhandledException(ConnectionDropErrorMessage).Once()
                .Then().Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = BadGatewayErrorMessage)]
        public void BecauseOf502ThenFails()
        {
            TestService
                .FailWithUnhandledException(BadGatewayErrorMessage).Once()
                .Then().Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

    }
}