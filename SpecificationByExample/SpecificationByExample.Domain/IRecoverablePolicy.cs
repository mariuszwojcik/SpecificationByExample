using System;

namespace SpecificationByExample.Domain
{
    public interface IRecoverablePolicy<TResult>
    {
        TResult Run(Func<TResult> func);
    }
}