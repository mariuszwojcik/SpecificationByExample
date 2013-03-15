using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace SpecificationByExample.Domain.UnitTests.SpecByExample.CommunicationErrorRecoveryPolicyTests
{
    [TestFixture]
    public class WhenOperationFailsWithUnsupportedException : CommunicationErrorRecoveryPolicyTestBase
    {
        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = TimedOutErrorMessage)]
        public void BecauseOfTimeoutThenRetiresFails()
        {
            TestService.Stub(i => i.Foo()).Throw(new Exception(TimedOutErrorMessage)).Repeat.Once();
            TestService.Stub(i => i.Foo()).Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = ConnectionDropErrorMessage)]
        public void BecauseOfConnectionDropsThenFails()
        {
            TestService.Stub(i => i.Foo()).Throw(new Exception(ConnectionDropErrorMessage)).Repeat.Once();
            TestService.Stub(i => i.Foo()).Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = BadGatewayErrorMessage)]
        public void BecauseOf502ThenFails()
        {
            TestService.Stub(i => i.Foo()).Throw(new Exception(BadGatewayErrorMessage)).Repeat.Once();
            TestService.Stub(i => i.Foo()).Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

    }
}