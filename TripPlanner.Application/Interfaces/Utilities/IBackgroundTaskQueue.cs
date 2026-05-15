using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces.Utilities
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueAsync(PlaceDetailsGenerationJob job, CancellationToken cancellationToken = default);
        ValueTask<PlaceDetailsGenerationJob> DequeueAsync(CancellationToken cancellationToken);
    }
}
