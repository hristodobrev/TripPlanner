namespace TripPlanner.Application.Interfaces.Providers
{
    public interface IImageProvider
    {
        Task<string?> GetImageUrlAsync(string placeName, CancellationToken cancellationToken);
    }
}
