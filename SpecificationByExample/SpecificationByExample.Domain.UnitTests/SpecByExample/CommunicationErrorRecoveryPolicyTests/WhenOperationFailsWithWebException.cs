using System.Net;
using NUnit.Framework;
using Rhino.Mocks;

namespace SpecificationByExample.Domain.UnitTests.SpecByExample.CommunicationErrorRecoveryPolicyTests
{
    [TestFixture]
    public class WhenOperationFailsWithWebException : CommunicationErrorRecoveryPolicyTestBase
    {
        [Test]
        public void BecauseOfTimeoutThenRetiresIt()
        {
            TestService.Stub(i => i.Foo()).Throw(new WebException(TimedOutErrorMessage)).Repeat.Once();
            TestService.Stub(i => i.Foo()).Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

        [Test]
        public void BecauseOfConnectionDropsThenRetiresIt()
        {
            TestService.Stub(i => i.Foo()).Throw(new WebException(ConnectionDropErrorMessage)).Repeat.Once();
            TestService.Stub(i => i.Foo()).Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

        [Test]
        public void BecauseOf502ThenRetires()
        {
            TestService.Stub(i => i.Foo()).Throw(new WebException(BadGatewayErrorMessage)).Repeat.Once();
            TestService.Stub(i => i.Foo()).Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

    }
}