namespace TripPlanner.Application.Interfaces
{
    public interface IImageProvider
    {
        Task<string?> GetImageUrlAsync(string placeName, CancellationToken cancellationToken);
    }
}
