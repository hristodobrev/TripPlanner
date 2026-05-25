using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.API.Tests.Utilities;

public static class IntegrationTestHelpers
{
    public static async Task<string> RegisterAndGetTokenAsync(
        this HttpClient client,
        string? email = null)
    {
        email ??= $"test-{Guid.NewGuid():N}@example.com";

        var response = await client.PostAsJsonAsync("/api/Auth/register", new RegisterRequest
        {
            Email = email,
            Password = "Password123!",
            FirstName = "Integration",
            LastName = "Tester"
        });

        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth.Should().NotBeNull();

        return auth!.AccessToken;
    }

    public static void UseBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public static async Task<Guid> CreateTripAsync(this HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/Trips", new TripRequest
        {
            Name = "Integration test trip",
            Description = "Created from integration test",
            PlaceId = $"fake-destination-{Guid.NewGuid():N}",
            DestinationName = "Paris",
            DestinationCountry = "France",
            StartDate = DateTime.UtcNow.Date.AddDays(10),
            EndDate = DateTime.UtcNow.Date.AddDays(14)
        });

        response.EnsureSuccessStatusCode();

        var tripId = await response.Content.ReadFromJsonAsync<Guid>();
        tripId.Should().NotBeEmpty();

        return tripId;
    }

    public static async Task<Guid> GetUserIdBySearchAsync(this HttpClient client, string email)
    {
        var response = await client.GetAsync($"/api/Users/search/{email}");
        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        users.Should().NotBeNull();

        return users!.Single(x => x.Email == email).Id;
    }
}