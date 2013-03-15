using System;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using Rhino.Mocks;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;
using SpecificationByExample.Domain.UnitTests.SpecByExample.CommunicationErrorRecoveryPolicyTests;

namespace SpecificationByExample.Domain.UnitTests.Dsl.extensions
{
    public static class MockSetupExtensions
    {
        private enum SimulationMode { GoogleAdWords, BingAds }

        [ThreadStatic] private static SimulationMode _simulatationMode;

        public static ITestService SimulateBingAds(this ITestService service)
        {
            _simulatationMode = SimulationMode.BingAds;
            return service;
        }

        public static ITestService SimulateGoogleAdWords(this ITestService service)
        {
            _simulatationMode = SimulationMode.GoogleAdWords;
            return service;
        }
        
        public static IMethodOptions<int> FailWithTimeOut(this ITestService service)
        {
            return service.FailWithSimulatedException(CommunicationErrorRecoveryPolicyTestBase.TimedOutErrorMessage);
        }

        public static IMethodOptions<int> FailWithConnectionDrop(this ITestService service)
        {
            return service.FailWithSimulatedException(CommunicationErrorRecoveryPolicyTestBase.ConnectionDropErrorMessage);
        }

        public static IMethodOptions<int> FailWithBadGateway(this ITestService service)
        {
            return service.FailWithSimulatedException(CommunicationErrorRecoveryPolicyTestBase.BadGatewayErrorMessage);
        }

        private static IMethodOptions<int> FailWithSimulatedException(this ITestService service, string errorMessage)
        {
            var exception = _simulatationMode == SimulationMode.GoogleAdWords
                                ? new WebException(errorMessage) as Exception
                                : new CommunicationException(errorMessage);

            return service.Expect(i => i.Foo()).Throw(exception);
        }
        
        public static IMethodOptions<int> FailWithUnhandledException(this ITestService service, string errorMessage)
        {
            return service.Expect(i => i.Foo()).Throw(new  Exception(errorMessage));
        }
        

        public static IMethodOptions<int> Times(this IMethodOptions<int> method, int repeatTimes)
        {
            return method.Repeat.Times(repeatTimes);
        }

        public static IMethodOptions<int> Once(this IMethodOptions<int> method)
        {
            return method.Repeat.Once();
        }

        public static ITestService Then(this IMethodOptions<int> method)
        {
            var fi = typeof(MethodOptions<int>).GetField("proxy", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi == null)
                throw new InvalidOperationException("Unable to get proxy of mocked class.");
            
            return fi.GetValue(method) as ITestService;
        }
        
        public static IMethodOptions<int> Return(this ITestService service, int value)
        {
            return service.Stub(i => i.Foo()).Return(value).Once();
        }

    }
}