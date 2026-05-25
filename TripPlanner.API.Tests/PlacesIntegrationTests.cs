using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TripPlanner.API.Tests.Utilities;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Domain.Enums;

namespace TripPlanner.API.Tests;

public class PlacesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PlacesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddPlace_WithToken_ShouldAddPlaceToTrip()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());
        var tripId = await _client.CreateTripAsync();

        var placeId = await AddPlaceAsync(tripId, "fake-louvre", "Louvre Museum");

        placeId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdatePlaceStatus_WithToken_ShouldReturnOk()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());
        var tripId = await _client.CreateTripAsync();
        var placeId = await AddPlaceAsync(tripId, "fake-louvre", "Louvre Museum");

        var response = await _client.PatchAsJsonAsync($"/api/Places/{placeId}/status", new UpdatePlaceStatusRequest
        {
            Status = PlaceStatus.Visited
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReorderPlaces_WithToken_ShouldReturnUpdatedPlaces()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());
        var tripId = await _client.CreateTripAsync();

        var firstPlaceId = await AddPlaceAsync(tripId, "fake-place-1", "First place");
        var secondPlaceId = await AddPlaceAsync(tripId, "fake-place-2", "Second place");

        var response = await _client.PutAsJsonAsync("/api/Places/reorder", new ReorderPlacesRequest
        {
            TripId = tripId,
            SourceId = firstPlaceId,
            TargetId = secondPlaceId,
            DayNumber = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var places = await response.Content.ReadFromJsonAsync<List<TripPlaceResponse>>();
        places.Should().NotBeNull();
        places.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task DeletePlace_WithToken_ShouldReturnOk()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());
        var tripId = await _client.CreateTripAsync();
        var placeId = await AddPlaceAsync(tripId, "fake-louvre", "Louvre Museum");

        var response = await _client.DeleteAsync($"/api/Places/{placeId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> AddPlaceAsync(Guid tripId, string externalId, string name)
    {
        var response = await _client.PostAsJsonAsync("/api/Places", new AddPlaceRequest
        {
            TripId = tripId,
            ExternalId = externalId,
            Name = name
        });

        response.EnsureSuccessStatusCode();

        var placeId = await response.Content.ReadFromJsonAsync<Guid>();
        return placeId;
    }
}