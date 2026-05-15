namespace TripPlanner.Domain.DTOs
{
    public class UserDashboardSummaryDto
    {
        public int TripsCount { get; set; }
        public int VisitedPlacesCount { get; set; }
        public int PlannedPlacesCount { get; set; }
    }
}
