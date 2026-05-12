namespace TripPlanner.Application.Interfaces
{
    public interface IDescriptionProvider
    {
        Task<string?> GetDescriptionAsync(string placeName, string placeLocation, CancellationToken cancellationToken);
    }
}
