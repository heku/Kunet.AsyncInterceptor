﻿// Copyright (c) 2020 stakx
// License available at https://github.com/stakx/DynamicProxy.AsyncInterceptor/blob/master/LICENSE.md.

using Xunit;

namespace stakx.DynamicProxy.Tests
{
    public class GetNumberTaskAsyncWithWaitAndReturn : ClockBasedTextFixture
    {
        [Fact]
        public void GetNumberTaskAsync_that_completes_at_time_1_is_initially_not_completed()
        {
            var proxy = this.CreateInterfaceProxy<IGetNumber>(new WaitUntil(this.Clock, 1));

            var task = proxy.GetNumberTaskAsync();

            Assert.False(task.IsCompleted);
        }

        [Fact]
        public async Task GetNumberTaskAsync_that_completes_at_time_1_is_successfully_completed_at_time_1()
        {
            var proxy = this.CreateInterfaceProxy<IGetNumber>(new WaitUntil(this.Clock, 1));
            var task = proxy.GetNumberTaskAsync();

            this.Clock.AdvanceTo(1);
            await task;

            Assert.True(task.IsCompleted && task.Status == TaskStatus.RanToCompletion); // IsCompletedSuccessfully
        }

        [Fact]
        public async Task GetNumberTaskAsync_that_completes_at_time_1_completes_successfully_with_correct_result_at_time_1()
        {
            var proxy = this.CreateInterfaceProxy<IGetNumber>(new WaitUntil(this.Clock, 1, proceed: true),
                                                              new Return(42));
            var task = proxy.GetNumberTaskAsync();

            this.Clock.AdvanceTo(1);
            await task;

            Assert.True(task.IsCompleted && task.Status == TaskStatus.RanToCompletion); // IsCompletedSuccessfully
            Assert.Equal(42, task.Result);
        }
    }
}