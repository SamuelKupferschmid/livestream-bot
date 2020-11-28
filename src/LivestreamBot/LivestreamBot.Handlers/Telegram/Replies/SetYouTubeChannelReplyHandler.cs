using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using LivestreamBot.Auth;
using LivestreamBot.Livestream.Events;
using LivestreamBot.Persistance;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LivestreamBot.Handlers.Telegram.Replies
{
    public class SetYouTubeChannelReplyHandler : BaseBotUpdateHandler
    {
        private readonly ITableStorage<ChatAuthorization> tableStorage;
        private readonly ILivestreamEventProvider eventProvider;
        private readonly ITelegramBotClient botClient;

        public SetYouTubeChannelReplyHandler(ITableStorage<ChatAuthorization> tableStorage, ILivestreamEventProvider eventProvider, ITelegramBotClient botClient)
        {
            this.tableStorage = tableStorage;
            this.eventProvider = eventProvider;
            this.botClient = botClient;
        }

        protected override bool Predicate(BotUpdate update) => update.Update.Type == UpdateType.CallbackQuery && update.Update.CallbackQuery.Data.StartsWith("setchannel:");

        public override async Task HandleUpdate(BotUpdate update, CancellationToken cancellationToken)
        {
            var chatId = update.Update.CallbackQuery.Message.Chat.Id;
            var channelId = Regex.Match(update.Update.CallbackQuery.Data, "^setchannel: (.*)$").Groups[1].Value;
            var entity = this.tableStorage.Get().Where(auth => auth.ChatId == chatId).First();

            entity.ChannelId = channelId;
            await this.tableStorage.InsertOrMergeAsync(entity);

            await this.botClient.SendTextMessageAsync(new ChatId(chatId), "Merci für die Infos. Der YouTube Channel ist gesetzt. Nun weiss ich alles nötige. Folgende Celebrations werde ich ab sofort Aufsetzen:", cancellationToken: cancellationToken);

            foreach(var @event in eventProvider.GetWeeklyEvents())
            {
                var localStart = DateTime.Today + @event.LocalEventStart;
                var streamStart = DateTime.Today + @event.LivestreamEventStart;

                await this.botClient.SendTextMessageAsync(new ChatId(chatId), $"{@event.Identifier} - {localStart:HH:mm} (Start Livestream:{streamStart:HH:mm})", cancellationToken: cancellationToken);
            }
            

            await Task.Delay(10000, cancellationToken);
            await this.botClient.SendTextMessageAsync(new ChatId(chatId), "PS: Noch ein Gruss von Paulus! Er hängt gerade bei mir. Natürlich mit 1.5m Abstand.🤙", cancellationToken: cancellationToken);
            await Task.Delay(2000, cancellationToken);
            await this.botClient.SendTextMessageAsync(new ChatId(chatId), $"#{update.Update.CallbackQuery.From.FirstName}4President", cancellationToken: cancellationToken);
        }

    }
}
