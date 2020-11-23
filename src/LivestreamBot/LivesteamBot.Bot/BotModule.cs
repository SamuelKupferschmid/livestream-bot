
using LivestreamBot.Bot.Subscriptions;
using LivestreamBot.Core.DI;

using SimpleInjector;

using System;
using System.Collections.Generic;
using System.Reflection;

using Telegram.Bot;

namespace LivestreamBot.Bot
{
    public class BotModule : IModule
    {

        public void Register(Container container, IList<Assembly> assemblies)
        {
            container.Register<ITelegramBotClient>(() =>
            {
                var token = Environment.GetEnvironmentVariable("TelegramToken");
                return new TelegramBotClient(token);
            });

            container.Register<ITelegramBotInfo, TelegramBotInfo>();
            container.Register<ITelegramBotSubscriptionService, TelegramBotSubscriptionService>();
        }
    }
}
