using System;
using NUnit.Framework;

namespace SpecificationByExample.Domain.UnitTests.Dsl.extensions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExpectedArgumentExceptionAttribute : ExpectedExceptionAttribute
    {
        public string ParameterName { get; set; }

        public ExpectedArgumentExceptionAttribute(string parameterName, string expectedMessage) :
            this(typeof(ArgumentException), parameterName, expectedMessage)
        {
        }

        protected ExpectedArgumentExceptionAttribute(Type exceptionType, string parameterName, string expectedMessage)
            : base(exceptionType)
        {
            ParameterName = parameterName;
            ExpectedMessage = string.Format("{0}\r\nParameter name: {1}", expectedMessage, parameterName);
        }
    }
}