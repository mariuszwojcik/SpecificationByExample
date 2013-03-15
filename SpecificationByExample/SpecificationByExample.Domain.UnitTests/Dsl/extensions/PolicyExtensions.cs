using System;
using System.Diagnostics;

namespace SpecificationByExample.Domain.UnitTests.Dsl.extensions
{
    public static class PolicyExtensions
    {
        public static TimeSpan RunAndMeasureExecutionTime(this CommunicationErrorRecoveryPolicy<int> policy, Func<int> func)
        {
            var watch = Stopwatch.StartNew();
            policy.Run(func);
            watch.Stop();

            return watch.Elapsed;
        }
    }
}