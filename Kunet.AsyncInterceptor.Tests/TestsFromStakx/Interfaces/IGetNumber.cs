// Copyright (c) 2020 stakx
// License available at https://github.com/stakx/DynamicProxy.AsyncInterceptor/blob/master/LICENSE.md.

namespace stakx.DynamicProxy.Tests
{
    public interface IGetNumber
    {
        int GetNumber();

        Task<int> GetNumberTaskAsync();

        ValueTask<int> GetNumberValueTaskAsync();
    }
}