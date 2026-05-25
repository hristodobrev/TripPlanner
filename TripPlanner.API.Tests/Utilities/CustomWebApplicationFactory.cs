using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TripPlanner.Application.Interfaces.Providers;

namespace TripPlanner.API.Tests.Utilities;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTesting");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Server=(localdb)\\mssqllocaldb;Database=TripPlannerIntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;",

                ["Jwt:Issuer"] = "TripPlanner.Tests",
                ["Jwt:Audience"] = "TripPlanner.Tests",
                ["Jwt:Key"] = "super-secret-test-key-super-secret-test-key",

                ["AiService:BaseUrl"] = "http://localhost:9999",
                ["GooglePlaces:ApiKey"] = "fake-google-key",
                ["UNSPLASH_ACCESS_KEY"] = "fake-unsplash-key"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IPlaceProvider>();
            services.AddScoped<IPlaceProvider, FakePlaceProvider>();

            services.RemoveAll<IPlaceRecommendationsProvider>();
            services.AddScoped<IPlaceRecommendationsProvider, FakePlaceRecommendationsProvider>();

            services.RemoveAll<IHostedService>();
        });
    }
}