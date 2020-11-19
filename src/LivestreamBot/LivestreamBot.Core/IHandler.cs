using System;
using System.Threading;
using System.Threading.Tasks;

namespace LivestreamBot.Core
{
    public interface IHandler<T>
    {
        Task Handle(T @event, CancellationToken cancellationToken);
    }
}
