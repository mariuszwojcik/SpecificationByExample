using System;

namespace SpecificationByExample.Domain.UnitTests.Dsl.extensions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExpectedArgumentOutOfRangeExceptionAttribute : ExpectedArgumentExceptionAttribute
    {
        public ExpectedArgumentOutOfRangeExceptionAttribute(string parameterName)
            : this(parameterName, "Specified argument was out of the range of valid values.")
        {
        }

        public ExpectedArgumentOutOfRangeExceptionAttribute(string parameterName, string expectedMessage) :
            base(typeof(ArgumentOutOfRangeException), parameterName, expectedMessage)
        {
        }
    }
}