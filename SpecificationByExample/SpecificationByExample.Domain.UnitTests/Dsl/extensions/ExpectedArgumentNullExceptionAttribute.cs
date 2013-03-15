using System;

namespace SpecificationByExample.Domain.UnitTests.Dsl.extensions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExpectedArgumentNullExceptionAttribute : ExpectedArgumentExceptionAttribute
    {
        public ExpectedArgumentNullExceptionAttribute(string parameterName, string expectedMessage)
            : base(typeof(ArgumentNullException), parameterName, expectedMessage)
        {
            ExpectedMessage = string.Format("{0}\r\nParameter name: {1}", expectedMessage, parameterName);
        }

        public ExpectedArgumentNullExceptionAttribute(string parameterName)
            : this(parameterName, "Value cannot be null.")
        {
        }
    }
}