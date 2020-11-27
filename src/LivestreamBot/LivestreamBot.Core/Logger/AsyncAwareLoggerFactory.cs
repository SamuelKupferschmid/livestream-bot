using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Microsoft.Extensions.Logging;

namespace LivestreamBot.Core.Logger
{
    public interface IAsyncAwareLoggerFactory : ILoggerFactory
    {
        ILoggerFactory Factory { get; set; }
    }

    public class AsyncAwareLoggerFactory : IAsyncAwareLoggerFactory
    {
        private readonly AsyncLocal<ILoggerFactory> innerFactory = new AsyncLocal<ILoggerFactory>();

        public ILoggerFactory Factory { get => innerFactory.Value; set => innerFactory.Value = value; }

        public void AddProvider(ILoggerProvider provider)
        {
            innerFactory.Value.AddProvider(provider);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return innerFactory.Value.CreateLogger(categoryName);
        }

        public void Dispose()
        {
            innerFactory.Value.Dispose();
        }
    }
}
