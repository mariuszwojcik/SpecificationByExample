using NUnit.Framework;
using Rhino.Mocks;
using SpecificationByExample.Domain.UnitTests.Dsl.extensions;

namespace SpecificationByExample.Domain.UnitTests.Dsl
{
    [TestFixture]
    public class WhenConnectingWithBingAdsAndOperationsFails : CommunicationErrorRecoveryPolicyTestBase
    {
        [Test]
        public void BecauseOfTimeoutThenRetires()
        {
            TestService
                .SimulateBingAds()
                .FailWithTimeOut().Once()
                .Then().Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

        [Test]
        public void BecauseOfConnectionDropsThenRetires()
        {
            TestService
                .SimulateBingAds()
                .FailWithConnectionDrop().Once()
                .Then().Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

        [Test]
        public void BecauseOf502ThenRetires()
        {
            TestService
                .SimulateBingAds()
                .FailWithBadGateway().Once()
                .Then().Return(9);

            var result = Policy.Run(() => TestService.Foo());

            Assert.AreEqual(9, result);
            TestService.VerifyAllExpectations();
        }

    }
}