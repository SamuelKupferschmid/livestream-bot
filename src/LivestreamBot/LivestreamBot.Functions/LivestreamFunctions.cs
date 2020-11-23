using System.Threading;
using System.Threading.Tasks;

using LivestreamBot.Functions;
using LivestreamBot.Livestream;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ChurchLiveStreamBot
{
    public class LivestreamFunctions
    {
        [FunctionName(nameof(LivestreamTimeTrigger))]
        public async Task LivestreamTimeTrigger([TimerTrigger("0 * * * * *")] TimerInfo timer, ILogger log, CancellationToken cancellationToken)
        {
            await FunctionsContainer.Mediator.Send(new LivestreamTimeTriggerRequest(), cancellationToken);
        }


    }
}
