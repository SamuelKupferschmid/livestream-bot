using FakeItEasy;

using LivestreamBot.Core.Environment;
using LivestreamBot.Handlers.Telegram.Commands;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using Xunit;

namespace LivestreamBot.Tests.Handlers.Telegram
{
    public class AuthorizeBotCommandHandlerTests
    {
        private readonly AuthorizeBotCommandHandler testee;
        private readonly ITelegramBotClient botClient;
        private readonly IAppConfig appConfig;
        public AuthorizeBotCommandHandlerTests()
        {
            botClient = A.Fake<ITelegramBotClient>();
            appConfig = A.Fake<IAppConfig>();
            testee = new AuthorizeBotCommandHandler(botClient, appConfig);
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnsLoginButtonAsync()
        {
            A.CallTo(() => appConfig.Host).Returns("host");

            var chatId = 42L;

            var update = new Message
            {
                Chat = new Chat
                {
                    Id = chatId
                }
            };

            await testee.Handle(update, CancellationToken.None);

            A.CallTo(() => botClient.SendTextMessageAsync(A<ChatId>.That.Matches(chat => chat.Identifier == chatId),
                                                          A<string>.Ignored,
                                                          A<ParseMode>.Ignored,
                                                          A<bool>.Ignored,
                                                          A<bool>.Ignored,
                                                          A<int>.Ignored,
                                                          A<IReplyMarkup>.Ignored,
                                                          A<CancellationToken>.Ignored))
                .MustHaveHappened();
        }
    }
}
