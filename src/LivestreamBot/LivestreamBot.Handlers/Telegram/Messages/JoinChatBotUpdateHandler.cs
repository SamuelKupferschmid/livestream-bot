
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace LivestreamBot.Handlers.Telegram
{
    public class JoinChatBotUpdateHandler : BaseBotUpdateHandler
    {
        private readonly ITelegramBotClient client;

        public JoinChatBotUpdateHandler(ITelegramBotClient client)
        {
            this.client = client;
        }

        protected override bool Predicate(BotUpdate update) => update.Update.Type == UpdateType.Message && update.Update.Message.Type == MessageType.ChatMembersAdded && update.Update.Message.NewChatMembers.Any(m => m.Id == client.BotId);

        public override async Task HandleUpdate(BotUpdate update, CancellationToken cancellationToken)
        {
            var message = @"Hey zusammen, ich bin Phillipus der Chatbot des Technik Teams. Meine Aufgabe ist, den Betrieb des Livestreams sicherzustellen.
Be blessed!";
            await this.client.SendTextMessageAsync(update.Update
                .Message.Chat.Id, message, ParseMode.Markdown, cancellationToken: cancellationToken);
        }
    }
}