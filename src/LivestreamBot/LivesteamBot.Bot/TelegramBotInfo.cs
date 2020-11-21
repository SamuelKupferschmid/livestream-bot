
using System;

namespace LivesteamBot.Bot
{
    public interface ITelegramBotInfo
    {
        public long OwnerChatId { get; set; }
    }

    public class TelegramBotInfo : ITelegramBotInfo
    {
        public TelegramBotInfo()
        {
            OwnerChatId = long.Parse(Environment.GetEnvironmentVariable("TelegramOwner"));
        }

        public long OwnerChatId { get; set; }
    }
}
