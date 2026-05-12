namespace TripPlanner.Application.Models
{
    public class PlaceDetailsGenerationJob
    {
        public Guid Id { get; set; }
        public string PlaceName { get; set; } = null!;
        public string PlaceLocation { get; set; } = null!;
    }
}
