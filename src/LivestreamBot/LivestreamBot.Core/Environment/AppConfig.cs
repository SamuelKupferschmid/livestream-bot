using System;
using System.Collections.Generic;
using System.Text;

namespace LivestreamBot.Core.Environment
{
    public interface IAppConfig
    {
        string Host { get; }
    }
    public class AppConfig : IAppConfig
    {
        public string Host => System.Environment.GetEnvironmentVariable(nameof(Host));
    }
}
