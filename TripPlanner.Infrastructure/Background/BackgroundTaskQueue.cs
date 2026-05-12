using System.Threading.Channels;
using TripPlanner.Application.Interfaces.Background;
using TripPlanner.Application.Models;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<PlaceDetailsGenerationJob> _queue;

    public BackgroundTaskQueue()
    {
        _queue = Channel.CreateUnbounded<PlaceDetailsGenerationJob>();
    }

    public ValueTask QueueAsync(PlaceDetailsGenerationJob job, CancellationToken cancellationToken = default)
    {
        return _queue.Writer.WriteAsync(job, cancellationToken);
    }

    public ValueTask<PlaceDetailsGenerationJob> DequeueAsync(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAsync(cancellationToken);
    }
}