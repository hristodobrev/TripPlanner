using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TripPlanner.API.Tests.Utilities;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Domain.Enums;

namespace TripPlanner.API.Tests;

public class TripSharesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TripSharesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTripById_WhenUserIsNotOwnerOrShared_ShouldReturnForbidden()
    {
        var ownerToken = await _client.RegisterAndGetTokenAsync();
        _client.UseBearerToken(ownerToken);
        var tripId = await _client.CreateTripAsync();

        var otherClient = _client;
        otherClient.DefaultRequestHeaders.Authorization = null;
        otherClient.UseBearerToken(await otherClient.RegisterAndGetTokenAsync());

        var response = await otherClient.GetAsync($"/api/Trips/{tripId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ShareTrip_WithAnotherUser_ShouldAllowSharedUserToOpenTrip()
    {
        var ownerEmail = $"owner-{Guid.NewGuid():N}@example.com";
        var sharedEmail = $"shared-{Guid.NewGuid():N}@example.com";

        var ownerToken = await _client.RegisterAndGetTokenAsync(ownerEmail);
        _client.UseBearerToken(ownerToken);
        var tripId = await _client.CreateTripAsync();

        var sharedToken = await _client.RegisterAndGetTokenAsync(sharedEmail);

        _client.UseBearerToken(ownerToken);
        var sharedUserId = await _client.GetUserIdBySearchAsync(sharedEmail);

        var shareResponse = await _client.PostAsJsonAsync($"/api/trips/{tripId}/shares", new AddTripShareRequest
        {
            UserId = sharedUserId,
            Permission = TripPermission.ReadOnly
        });

        shareResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        _client.UseBearerToken(sharedToken);
        var getResponse = await _client.GetAsync($"/api/Trips/{tripId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var trip = await getResponse.Content.ReadFromJsonAsync<GetTripResponse>();
        trip!.Shared.Should().BeTrue();
        trip.SharedPermission.Should().Be(TripPermission.ReadOnly);
    }

    [Fact]
    public async Task ShareTrip_WhenUserIsNotOwner_ShouldReturnForbidden()
    {
        var ownerToken = await _client.RegisterAndGetTokenAsync();
        _client.UseBearerToken(ownerToken);
        var tripId = await _client.CreateTripAsync();

        var otherToken = await _client.RegisterAndGetTokenAsync();
        _client.UseBearerToken(otherToken);

        var response = await _client.PostAsJsonAsync($"/api/trips/{tripId}/shares", new AddTripShareRequest
        {
            UserId = Guid.NewGuid(),
            Permission = TripPermission.ReadOnly
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}