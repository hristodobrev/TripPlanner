using FluentAssertions;
using Moq;
using TripPlanner.Application.Interfaces.Providers;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Models;
using TripPlanner.Application.Services;
using TripPlanner.Domain.DTOs;

namespace TripPlanner.Application.Tests.Services;

public class PlaceRecommendationsServiceTests
{
    private readonly Mock<ITripRepository> _tripRepository = new();
    private readonly Mock<IPlaceRecommendationsProvider> _provider = new();

    private PlaceRecommendationsService CreateService() =>
        new(_tripRepository.Object, _provider.Object);

    [Fact]
    public async Task GetPlaceRecommendationsAsync_ShouldSendVisitedPlacesToProviderAndReturnRecommendations()
    {
        var userId = Guid.NewGuid();

        _tripRepository.Setup(x => x.GetTopDestinations(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<VisitedPlaceDto>
            {
                new() { Name = "Paris", Country = "France", Description = "Culture and museums" },
                new() { Name = "Rome", Country = "Italy", Description = "History and food" }
            });

        _provider.Setup(x => x.GetPlaceRecommendationsAsync(
                It.IsAny<List<PlaceRecommendationRequest>>(),
                3,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlaceRecommendationResult>
            {
                new()
                {
                    Name = "Barcelona",
                    Country = "Spain",
                    Description = "Architecture and beaches",
                    PlaceId = "barcelona-place",
                    ImageUrl = "https://example.com/image.jpg"
                }
            });

        var result = (await CreateService().GetPlaceRecommendationsAsync(userId, CancellationToken.None)).ToList();

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Barcelona");
        result[0].Country.Should().Be("Spain");
        result[0].PlaceId.Should().Be("barcelona-place");

        _provider.Verify(x => x.GetPlaceRecommendationsAsync(
            It.Is<List<PlaceRecommendationRequest>>(r =>
                r.Count == 2 &&
                r[0].name == "Paris" &&
                r[1].name == "Rome"),
            3,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}