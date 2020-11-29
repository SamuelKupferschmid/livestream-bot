using MediatR;
using LivestreamBot.Livestream.Notifications;
using System.Threading.Tasks;
using System.Threading;
using LivestreamBot.Livestream.Events;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;
using LivestreamBot.Livestream.Scheduling;

namespace LivestreamBot.Handlers.Livestream.Scheduling
{
    public class LivestreamEventSetupTimeTriggerNotificationHandler : INotificationHandler<LivestreamTimeTriggerNotification>
    {
        private readonly ILivestreamEventProvider eventProvider;
        private readonly TimeZoneInfo timezoneInfo;
        private readonly ITelegramBotClient botClient;
        private readonly ISchedulingService schedulingService;

        public LivestreamEventSetupTimeTriggerNotificationHandler(ILivestreamEventProvider eventProvider, TimeZoneInfo timezoneInfo, ITelegramBotClient botClient, ISchedulingService schedulingService)
        {
            this.eventProvider = eventProvider;
            this.timezoneInfo = timezoneInfo;
            this.botClient = botClient;
            this.schedulingService = schedulingService;
        }


        public async Task Handle(LivestreamTimeTriggerNotification notification, CancellationToken cancellationToken)
        {
            foreach (var @event in eventProvider.GetWeeklyEvents())
            {

                var lastTrigger = TimeZoneInfo.ConvertTimeFromUtc(notification.Last.ToUniversalTime(), timezoneInfo);
                var nextTrigger = TimeZoneInfo.ConvertTimeFromUtc(notification.Next.ToUniversalTime(), timezoneInfo);


                var dueAtLastTrigger = @event.GetLivestreamEventInfo(lastTrigger).untilNext < @event.LivestreamAnnouncmentLead;
                var dueBeforeNextTrigger = @event.GetLivestreamEventInfo(nextTrigger).untilNext < @event.LivestreamAnnouncmentLead;

                var triggerNow = !dueAtLastTrigger && dueBeforeNextTrigger;

                if (triggerNow)
                {

                    var eventStart = DateTime.Today + @event.LocalEventStart;
                    var streamStart = DateTime.Today + @event.LivestreamEventStart;

                    foreach (var (chatId, channelId) in this.schedulingService.GetEventSubscriptions(@event))
                    {

                        var buttons = new InlineKeyboardButton[]{
                        new InlineKeyboardButton
                        {
                            Text = $"Ja klar. Start um {streamStart:HH:mm}",
                            CallbackData = $"createbroadcast: {@event.Identifier} {channelId}",
                        }/*, new InlineKeyboardButton
                        {
                            Text = "Ja, aber bitte noch private",
                            CallbackData = $"createbroadcast: {@event.Identifier} {channelId} private",
                        }*/
                    };

                        var button1 = new InlineKeyboardMarkup(buttons.Select(button => new InlineKeyboardButton[] { button }));

                        var message = $"Moin. {@event.Identifier}-Celebration steht vor der Türe. Celebration Start um {eventStart:HH:mm}. Livestream startet um {streamStart:HH:mm}. Soll ich euch einen Event auf Youtube erstellen?";

                        await botClient.SendTextMessageAsync(new ChatId(chatId), message, replyMarkup: button1, cancellationToken: cancellationToken);
                    }
                }
            }
        }
    }
}
