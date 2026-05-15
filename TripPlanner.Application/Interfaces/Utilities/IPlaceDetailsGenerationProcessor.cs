using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces.Utilities
{
    public interface IPlaceDetailsGenerationProcessor
    {
        Task ProcessAsync(PlaceDetailsGenerationJob job, CancellationToken cancellationToken);
    }
}
