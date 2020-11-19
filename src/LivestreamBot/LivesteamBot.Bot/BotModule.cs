using LivestreamBot.Core;

using Microsoft.Extensions.Configuration;

using SimpleInjector;

using System;

using Telegram.Bot;

namespace LivesteamBot.Bot
{
    public class BotModule : IModule
    {
        public void Register(Container container)
        {
            container.Register<ITelegramBotClient>(() =>
            {
                var token = Environment.GetEnvironmentVariable("TelegramToken");
                Console.WriteLine(token);
                return new TelegramBotClient(token);
            });

            container.Register<ITelegramBotInfo, TelegramBotInfo>();
        }
    }
}
