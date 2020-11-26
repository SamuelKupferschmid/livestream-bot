
using LivestreamBot.Core.Environment;

using System;

namespace LivestreamBot.Bot
{
    public interface ITelegramBotInfo
    {
        public long OwnerChatId { get; set; }
    }

    public class TelegramBotInfo : ITelegramBotInfo
    {
        public TelegramBotInfo(IAppConfig appConfig)
        {
            OwnerChatId = appConfig.TelegramOwner;
        }

        public long OwnerChatId { get; set; }
    }
}
