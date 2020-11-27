using Microsoft.Azure.Cosmos.Table;

using System;

namespace LivestreamBot.Auth
{
    public class ChatAuthorizations : TableEntity
    {
        private long chatId;

        public ChatAuthorizations()
        {
            PartitionKey = nameof(ChatAuthorizations);
        }

        public long ChatId
        {
            get => chatId; set
            {
                RowKey = value.ToString();
                chatId = value;
            }
        }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long? ExpiresInSeconds { get; set; }
        public DateTime IssuedUtc { get; set; }
        public string Scope { get; set; }
        public string TokenType { get; set; }
        public string ChannelId { get; set; }

    }
}
