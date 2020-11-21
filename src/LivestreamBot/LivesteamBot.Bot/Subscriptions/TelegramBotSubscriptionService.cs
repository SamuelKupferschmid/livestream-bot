using LivestreamBot.Persistance;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LivestreamBot.Bot.Subscriptions
{
    public interface ITelegramBotSubscriptionService
    {
        Task Subscribe(long chatId, string subscriptionName, CancellationToken cancellationToken);
        Task UnsubscribeAll(long chatId, CancellationToken cancellationToken);
        Task<IEnumerable<long>> GetSubcribers(string subscriptionName, CancellationToken cancellationToken);
    }
    public class TelegramBotSubscriptionService : ITelegramBotSubscriptionService
    {
        private readonly ITableStorage<NotificationSubscription> tableStorage;

        public TelegramBotSubscriptionService(ITableStorage<NotificationSubscription> tableStorage)
        {
            this.tableStorage = tableStorage;
        }
        public Task<IEnumerable<long>> GetSubcribers(string subscriptionName, CancellationToken cancellationToken)
        {
            var chats = tableStorage.Get().Where(sub => sub.NotificationType == subscriptionName).Select(sub => sub.ChatId).AsEnumerable();
            return Task.FromResult(chats);
        }

        public async Task Subscribe(long chatId, string subscriptionName, CancellationToken cancellationToken)
        {
            await tableStorage.InsertOrMergeAsync(new NotificationSubscription(chatId, subscriptionName));
        }

        public async Task UnsubscribeAll(long chatId, CancellationToken cancellationToken)
        {
            foreach (var chat in tableStorage.Get().Where(sub => sub.ChatId == chatId))
            {
                await tableStorage.DeleteAsync(chat, cancellationToken);
            }
        }
    }
}
