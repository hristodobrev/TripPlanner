using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TripPlanner.API.Tests.Utilities;
using TripPlanner.Application.DTOs.Response;

namespace TripPlanner.API.Tests;

public class PlaceSearchIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PlaceSearchIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Autocomplete_WithToken_ShouldReturnSuggestions()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());

        var response = await _client.GetAsync("/api/PlaceSearch/autocomplete/paris");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<PlaceAutoCompleteResponse>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task TextSearch_WithToken_ShouldReturnPlaces()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());

        var response = await _client.GetAsync("/api/PlaceSearch/48.8566/2.3522/museum");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<PlaceTextSearchResponse>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPlace_WithToken_ShouldReturnPlaceDetails()
    {
        _client.UseBearerToken(await _client.RegisterAndGetTokenAsync());

        var response = await _client.GetAsync("/api/PlaceSearch/fake-paris");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<GetPlaceResponse>();
        result!.ExternalId.Should().Be("fake-paris");
    }
}