using TripPlanner.Application.Models;

namespace TripPlanner.Application.Interfaces
{
    public interface IPlaceDetailsGenerationProcessor
    {
        Task ProcessAsync(PlaceDetailsGenerationJob job, CancellationToken cancellationToken);
    }
}
