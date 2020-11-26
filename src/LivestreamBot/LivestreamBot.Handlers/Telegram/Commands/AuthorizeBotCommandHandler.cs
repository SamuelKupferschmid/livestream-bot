
using LivestreamBot.Core.Environment;

using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace LivestreamBot.Handlers.Telegram.Commands
{
    public class AuthorizeBotCommandHandler : BaseBotCommandHandler
    {
        private readonly ITelegramBotClient client;
        private readonly IAppConfig appConfig;

        public AuthorizeBotCommandHandler(ITelegramBotClient client, IAppConfig appConfig)
        {
            this.client = client;
            this.appConfig = appConfig;
        }

        protected override string Command => "authorize";

        public override async Task Handle(Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var button = new InlineKeyboardButton[]{  new InlineKeyboardButton
                {
                    Text = $"Für Youtube authorisieren.",
                    CallbackData = "login",
                    Url = $"{appConfig.Host}/api/authorize/{chatId}"
                }
                };
            var reply = new InlineKeyboardMarkup(button);

            var text = @$"Wenn du mir den Zugriff auf dein Youtube Konto erlaubst, kann ich dir die Livestreams aufsetzen.
Natürlich nur mit Rücksprache in diesem Chat. 
Du gibst damit also mir, sowie diesem Chat hier viel Verantwortung.";

            await client.SendTextMessageAsync(new ChatId(chatId), text, replyMarkup: reply, cancellationToken: cancellationToken);
        }
    }
}