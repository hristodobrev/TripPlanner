namespace TripPlanner.Application.DTOs.Response
{
    public class TripPlaceResponse
    {
        public Guid Id { get; set; }
        public int? DayNumber { get; set; }
        public string Name { get; set; } = null!;
        public string? Note { get; set; }
    }
}
