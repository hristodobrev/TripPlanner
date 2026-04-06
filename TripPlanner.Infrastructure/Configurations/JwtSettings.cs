namespace TripPlanner.Infrastructure.Configurations
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string Key { get; set; } = null!;
        public int AccessTokenExpirationMinutes { get; set; }
    }
}
