using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using LivestreamBot.Auth.Google;
using LivestreamBot.Livestream.Events;
using LivestreamBot.Livestream.Scheduling;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LivestreamBot.Handlers.Telegram.Replies
{
    public class CreateBroadcastReplyHandler : BaseBotUpdateHandler
    {
        private readonly IYouTubeAuthorizationService youTubeAuthorization;
        private readonly ISchedulingService schedulingService;
        private readonly ILivestreamEventProvider eventProvider;
        private readonly TimeZoneInfo timezoneInfo;
        private readonly ITelegramBotClient botClient;

        public CreateBroadcastReplyHandler(ISchedulingService schedulingService, IYouTubeAuthorizationService youTubeAuthorization, TimeZoneInfo timezoneInfo, ILivestreamEventProvider eventProvider, ITelegramBotClient botClient)
        {
            this.schedulingService = schedulingService;
            this.youTubeAuthorization = youTubeAuthorization;
            this.timezoneInfo = timezoneInfo;
            this.eventProvider = eventProvider;
            this.botClient = botClient;
        }

        protected override bool Predicate(BotUpdate update) => update.Update.Type == UpdateType.CallbackQuery && update.Update.CallbackQuery.Data.StartsWith("createbroadcast:");

        public override async Task HandleUpdate(BotUpdate update, CancellationToken cancellationToken)
        {
            var chatId = update.Update.CallbackQuery.Message.Chat.Id;
            var groups = Regex.Match(update.Update.CallbackQuery.Data, @"^createbroadcast: (\S+) (\S+)").Groups;
            var eventIdentifier = groups[1].Value;
            var channelId = groups[2].Value;

            var @event = this.eventProvider.GetWeeklyEvents().First(ev => ev.Identifier == eventIdentifier);
            var broadcast = await schedulingService.ScheduleEvent(@event, channelId, chatId, @event.GetNext(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this.timezoneInfo)), cancellationToken);

            var url = $"https://www.youtube.com/watch?v={broadcast.Id}";
            await this.botClient.SendTextMessageAsync(new ChatId(chatId), $"Broadcast aufgesetzt: {url}", cancellationToken: cancellationToken);
        }
    }
}
