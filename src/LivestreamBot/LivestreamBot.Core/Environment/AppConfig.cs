using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LivestreamBot.Core.Environment
{
    public interface IAppConfig
    {
        string AzureWebJobsStorage { get; }
        string YoutubeApiKey { get; }
        string YoutubeChannelId { get; }
        string TelegramToken { get; }
        long TelegramOwner { get; }
        string Host { get; }
    }

    public class AppConfig : IAppConfig
    {
        public AppConfig(IConfiguration configuration)
        {
            foreach (var prop in typeof(AppConfig).GetProperties())
            {
                // set the backingfiekds
                var field = typeof(AppConfig).GetField($"<{prop.Name}>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var value = configuration.GetValue(prop.PropertyType, prop.Name);
                field.SetValue(this, value);
            }
        }

        public string AzureWebJobsStorage { get; }
        public string YoutubeApiKey { get; }
        public string YoutubeChannelId { get; }
        public string TelegramToken { get; }
        public long TelegramOwner { get; }
        public string Host { get; }
    }
}
