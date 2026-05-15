namespace TripPlanner.Application.Interfaces.Providers
{
    public interface IDescriptionProvider
    {
        Task<string?> GetDescriptionAsync(string placeName, string placeLocation, CancellationToken cancellationToken);
    }
}
