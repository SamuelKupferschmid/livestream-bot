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
        public async Task LivestreamTimeTrigger([TimerTrigger("0 */5 * * * *")] TimerInfo timer, ILogger log, CancellationToken cancellationToken)
        {
            await FunctionsContainer.Mediator.Publish(new LivestreamTimeTriggerNotification
            {
                Last = timer.ScheduleStatus.Last,
                Next = timer.ScheduleStatus.Next,
            }, cancellationToken);
        }


    }
}
