using System.Threading;
using LivestreamBot.Functions;
using LivestreamBot.Livestream;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ChurchLiveStreamBot
{
    public class LivestreamFunctions
    {
        [FunctionName(nameof(LivesteamTimeTrigger))]
        public async System.Threading.Tasks.Task LivesteamTimeTrigger([TimerTrigger("0 * * * * *")]TimerInfo timer, ILogger log, CancellationToken cancellationToken)
        {
            await FunctionsContainer.Mediator.Send(new LivestreamTimeTriggerRequest(), cancellationToken);
        }


    }
}
