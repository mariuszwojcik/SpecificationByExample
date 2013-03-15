using System;
using System.Net;
using System.ServiceModel;
using System.Threading;

namespace SpecificationByExample.Domain
{
    public class CommunicationErrorRecoveryPolicy<TResult> : IRecoverablePolicy<TResult>
    {
        private readonly int _allowedRetries;
        private readonly TimeSpan _waitBeforeRetry;

        public CommunicationErrorRecoveryPolicy(int allowedRetries, TimeSpan waitBeforeRetry)
        {
            if ((allowedRetries < 1) || (allowedRetries > 10))
                throw new ArgumentOutOfRangeException("allowedRetries", "Value must be between 1 and 10.");
            if ((waitBeforeRetry < TimeSpan.FromMilliseconds(50)) || (waitBeforeRetry > TimeSpan.FromMinutes(1)))
                throw new ArgumentOutOfRangeException("waitBeforeRetry", "Value must be between 50 milliseconds and 1 minute.");

            _allowedRetries = allowedRetries;
            _waitBeforeRetry = waitBeforeRetry;
        }

        public CommunicationErrorRecoveryPolicy()
            : this(5, TimeSpan.FromSeconds(15))
        {
        }

        public int AllowedRetries
        {
            get
            {
                return _allowedRetries;
            }
        }

        public TimeSpan WaitBeforeRetry
        {
            get
            {
                return _waitBeforeRetry;
            }
        }

        public TResult Run(Func<TResult> func)
        {
            var retryCounter = 0;
            while (true)
            {
                try
                {
                    return func.Invoke();
                }
                catch (Exception e)
                {
                    if ((retryCounter < _allowedRetries) && ShouldRetryOpertaion(e))
                    {
                        var waitTime = (int)Math.Pow(2, retryCounter++) * _waitBeforeRetry.Milliseconds;
                        Thread.Sleep(waitTime);
                        continue;
                    }

                    throw;
                }
            }
        }

        private static bool ShouldRetryOpertaion(Exception e)
        {
            return (e is WebException || e is CommunicationException)
                   && (e.Message == "The operation has timed out"
                       || e.Message == "The underlying connection was closed: A connection that was expected to be kept alive was closed by the server."
                       || e.Message == "The request failed with HTTP status 502: Bad Gateway.");
        }
    }
}