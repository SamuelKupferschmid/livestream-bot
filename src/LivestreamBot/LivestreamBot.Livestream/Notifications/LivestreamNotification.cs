
using Microsoft.Azure.Cosmos.Table;

using System;

namespace LivestreamBot.Livestream.Notifications
{
    public class LivestreamNotification : TableEntity
    {
        public LivestreamNotification()
        {
            PartitionKey = nameof(LivestreamNotification);
            RowKey = Guid.NewGuid().ToString();
        }


        public string Name { get; set; }
        public DateTime DateTime { get; set; }

        public string VideoId { get; set; }
    }
}
