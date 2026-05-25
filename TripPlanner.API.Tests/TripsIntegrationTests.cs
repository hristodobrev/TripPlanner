using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TripPlanner.API.Tests.Utilities;
using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.API.Tests;

public class TripsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TripsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTrip_WithToken_ShouldCreateTrip()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());

        var tripId = await _client.CreateTripAsync();

        tripId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetTrips_WithToken_ShouldReturnUserTrips()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());
        await _client.CreateTripAsync();

        var response = await _client.GetAsync("/api/Trips");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var trips = await response.Content.ReadFromJsonAsync<List<GetAllTripResponse>>();
        trips.Should().NotBeNull();
        trips.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetTripById_ForOwner_ShouldReturnTripDetails()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());
        var tripId = await _client.CreateTripAsync();

        var response = await _client.GetAsync($"/api/Trips/{tripId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var trip = await response.Content.ReadFromJsonAsync<GetTripResponse>();
        trip!.Id.Should().Be(tripId);
    }

    [Fact]
    public async Task GetRecommendations_WithToken_ShouldReturnRecommendations()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());

        var response = await _client.GetAsync("/api/Trips/recommendations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var recommendations = await response.Content.ReadFromJsonAsync<List<PlaceRecommendationsResponse>>();
        recommendations.Should().NotBeNull();
        recommendations.Should().NotBeEmpty();
    }
}