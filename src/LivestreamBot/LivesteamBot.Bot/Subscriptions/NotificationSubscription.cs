using Microsoft.Azure.Cosmos.Table;

namespace LivestreamBot.Bot.Subscriptions
{
    public class NotificationSubscription : TableEntity
    {
        public NotificationSubscription()
        {
            PartitionKey = nameof(NotificationSubscription);
        }

        public NotificationSubscription(long chatId, string notificationType)
        {
            ChatId = chatId;
            NotificationType = notificationType;
            RowKey = $"{ChatId}_{notificationType}";
            PartitionKey = nameof(NotificationSubscription);
        }

        public long ChatId { get; set; }
        public string NotificationType { get; set; }
    }
}
