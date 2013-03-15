using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace SpecificationByExample.Domain.UnitTests.SpecByExample.CommunicationErrorRecoveryPolicyTests
{
    public abstract class CommunicationErrorRecoveryPolicyTestBase
    {
        internal const string ConnectionDropErrorMessage = "The underlying connection was closed: A connection that was expected to be kept alive was closed by the server.";
        internal const string TimedOutErrorMessage = "The operation has timed out";
        internal const string BadGatewayErrorMessage = "The request failed with HTTP status 502: Bad Gateway.";


        protected ITestService TestService { get; private set; }
        protected CommunicationErrorRecoveryPolicy<int> Policy { get; private set; }

        [SetUp]
        public void TestSetUp()
        {
            TestService = MockRepository.GenerateMock<ITestService>();

            Policy = new CommunicationErrorRecoveryPolicy<int>(3, TimeSpan.FromMilliseconds(50));
        }
    }
}