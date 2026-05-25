using FluentAssertions;
using Moq;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.Exceptions;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Interfaces.Utilities;
using TripPlanner.Application.Models;
using TripPlanner.Application.Services;
using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.Tests.Services;

public class TripPlaceServiceTests
{
    private readonly Mock<ITripPlaceRepository> _tripPlaceRepository = new();
    private readonly Mock<IPlaceRepository> _placeRepository = new();
    private readonly Mock<ITripRepository> _tripRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IBackgroundTaskQueue> _backgroundTaskQueue = new();

    public TripPlaceServiceTests()
    {
        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _backgroundTaskQueue.Setup(x => x.QueueAsync(It.IsAny<PlaceDetailsGenerationJob>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);
    }

    private TripPlaceService CreateService() =>
        new(_tripPlaceRepository.Object, _placeRepository.Object, _tripRepository.Object, _unitOfWork.Object, _backgroundTaskQueue.Object);

    [Fact]
    public async Task AddAsync_WhenUserIsOwnerAndPlaceExists_ShouldAddTripPlace()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        var place = new Place { Id = Guid.NewGuid(), ExternalId = "place-1", Name = "Louvre", Description = "Museum", ImageUrl = "image.jpg" };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, userId));

        _placeRepository.Setup(x => x.GetByExternalIdAsync(place.ExternalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(place);

        TripPlace? addedTripPlace = null;

        _tripPlaceRepository.Setup(x => x.AddAsync(It.IsAny<TripPlace>(), It.IsAny<CancellationToken>()))
            .Callback<TripPlace, CancellationToken>((tripPlace, _) => addedTripPlace = tripPlace)
            .Returns(Task.CompletedTask);

        var request = new AddPlaceRequest { TripId = tripId, ExternalId = place.ExternalId, Name = "Louvre" };

        var result = await CreateService().AddAsync(request, userId, CancellationToken.None);

        result.Should().NotBeEmpty();
        addedTripPlace.Should().NotBeNull();
        addedTripPlace!.TripId.Should().Be(tripId);
        addedTripPlace.PlaceId.Should().Be(place.Id);

        _backgroundTaskQueue.Verify(x => x.QueueAsync(It.IsAny<PlaceDetailsGenerationJob>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenPlaceHasNoGeneratedDetails_ShouldQueueAiGenerationJob()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        var place = new Place { Id = Guid.NewGuid(), ExternalId = "place-1", Name = "Colosseum" };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, userId));

        _placeRepository.Setup(x => x.GetByExternalIdAsync(place.ExternalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(place);

        _tripPlaceRepository.Setup(x => x.AddAsync(It.IsAny<TripPlace>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var request = new AddPlaceRequest { TripId = tripId, ExternalId = place.ExternalId, Name = "Colosseum" };

        await CreateService().AddAsync(request, userId, CancellationToken.None);

        _backgroundTaskQueue.Verify(x => x.QueueAsync(
            It.Is<PlaceDetailsGenerationJob>(j => j.Id == place.Id && j.PlaceName == "Colosseum"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenUserHasEditShare_ShouldAllowAddingPlace()
    {
        var ownerId = Guid.NewGuid();
        var sharedUserId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        var place = new Place { Id = Guid.NewGuid(), ExternalId = "place-1", Name = "Museum", Description = "Description", ImageUrl = "image.jpg" };
        var trip = CreateTrip(tripId, ownerId);
        trip.TripShares = new List<TripShare> { new() { UserId = sharedUserId, Permission = TripPermission.Edit } };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>())).ReturnsAsync(trip);
        _placeRepository.Setup(x => x.GetByExternalIdAsync(place.ExternalId, It.IsAny<CancellationToken>())).ReturnsAsync(place);
        _tripPlaceRepository.Setup(x => x.AddAsync(It.IsAny<TripPlace>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var request = new AddPlaceRequest { TripId = tripId, ExternalId = place.ExternalId, Name = "Museum" };

        await CreateService().AddAsync(request, sharedUserId, CancellationToken.None);

        _tripPlaceRepository.Verify(x => x.AddAsync(It.IsAny<TripPlace>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenUserHasReadOnlyShare_ShouldThrowForbiddenException()
    {
        var ownerId = Guid.NewGuid();
        var sharedUserId = Guid.NewGuid();
        var tripId = Guid.NewGuid();
        var trip = CreateTrip(tripId, ownerId);
        trip.TripShares = new List<TripShare> { new() { UserId = sharedUserId, Permission = TripPermission.ReadOnly } };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>())).ReturnsAsync(trip);

        var request = new AddPlaceRequest { TripId = tripId, ExternalId = "place-1", Name = "Museum" };

        var act = async () => await CreateService().AddAsync(request, sharedUserId, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task RemoveAsync_WhenTripPlaceDoesNotExist_ShouldThrowNotFoundException()
    {
        _tripPlaceRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TripPlace?)null);

        var act = async () => await CreateService().RemoveAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ReorderAsync_WhenMovingPlaceForwardInSameDay_ShouldShiftPlacesAndUpdateSourceOrder()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var source = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Source",
            DayNumber = 1,
            Order = 1
        };

        var middle = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Middle",
            DayNumber = 1,
            Order = 2
        };

        var target = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Target",
            DayNumber = 1,
            Order = 3
        };

        var tripPlaces = new List<TripPlace> { source, middle, target };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, userId));

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(target.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(target);

        _tripPlaceRepository.Setup(x => x.GetByTripIdAndDayNumberAsync(tripId, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tripPlaces);

        var request = new ReorderPlacesRequest
        {
            TripId = tripId,
            SourceId = source.Id,
            TargetId = target.Id,
            DayNumber = 1
        };

        await CreateService().ReorderAsync(request, userId, CancellationToken.None);

        source.Order.Should().Be(3);
        middle.Order.Should().Be(1);
        target.Order.Should().Be(2);

        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReorderAsync_WhenMovingPlaceBackwardInSameDay_ShouldShiftPlacesAndUpdateSourceOrder()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var target = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Target",
            DayNumber = 1,
            Order = 1
        };

        var middle = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Middle",
            DayNumber = 1,
            Order = 2
        };

        var source = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Source",
            DayNumber = 1,
            Order = 3
        };

        var tripPlaces = new List<TripPlace> { target, middle, source };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, userId));

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(target.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(target);

        _tripPlaceRepository.Setup(x => x.GetByTripIdAndDayNumberAsync(tripId, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tripPlaces);

        var request = new ReorderPlacesRequest
        {
            TripId = tripId,
            SourceId = source.Id,
            TargetId = target.Id,
            DayNumber = 1
        };

        await CreateService().ReorderAsync(request, userId, CancellationToken.None);

        source.Order.Should().Be(1);
        target.Order.Should().Be(2);
        middle.Order.Should().Be(3);

        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReorderAsync_WhenMovingPlaceToAnotherDayWithoutTarget_ShouldMoveToEndOfDay()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var source = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Source",
            DayNumber = 1,
            Order = 2
        };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, userId));

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        _tripPlaceRepository.Setup(x => x.GetMaxOrderForDayAsync(tripId, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(4);

        var request = new ReorderPlacesRequest
        {
            TripId = tripId,
            SourceId = source.Id,
            TargetId = null,
            DayNumber = 2
        };

        await CreateService().ReorderAsync(request, userId, CancellationToken.None);

        source.DayNumber.Should().Be(2);
        source.Order.Should().Be(5);

        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReorderAsync_WhenSourcePlaceIsNotInTrip_ShouldThrowNotFoundException()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var source = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = Guid.NewGuid(),
            Name = "Source",
            DayNumber = 1,
            Order = 1
        };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, userId));

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        var request = new ReorderPlacesRequest
        {
            TripId = tripId,
            SourceId = source.Id,
            TargetId = null,
            DayNumber = 1
        };

        var act = async () => await CreateService().ReorderAsync(request, userId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ReorderAsync_WhenUserHasNoEditAccess_ShouldThrowForbiddenException()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, ownerId));

        var request = new ReorderPlacesRequest
        {
            TripId = tripId,
            SourceId = Guid.NewGuid(),
            TargetId = null,
            DayNumber = 1
        };

        var act = async () => await CreateService().ReorderAsync(request, otherUserId, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task ReorderAsync_WhenMovedPlaceOverlapsNextPlace_ShouldPushNextPlaceAfterMovedPlace()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var source = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Museum",
            DayNumber = 1,
            Order = 1,
            PlannedTime = new TimeOnly(10, 00),
            DurationMinutes = 90
        };

        var target = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Gallery",
            DayNumber = 1,
            Order = 2,
            PlannedTime = new TimeOnly(10, 30),
            DurationMinutes = 60
        };

        var tripPlaces = new List<TripPlace> { source, target };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, userId));

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(target.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(target);

        _tripPlaceRepository.Setup(x => x.GetByTripIdAndDayNumberAsync(tripId, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tripPlaces);

        var request = new ReorderPlacesRequest
        {
            TripId = tripId,
            SourceId = source.Id,
            TargetId = target.Id,
            DayNumber = 1
        };

        await CreateService().ReorderAsync(request, userId, CancellationToken.None);

        target.PlannedTime.Should().Be(new TimeOnly(10, 30));
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReorderAsync_WhenPlacesDoNotOverlap_ShouldKeepPlannedTimes()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var source = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Museum",
            DayNumber = 1,
            Order = 1,
            PlannedTime = new TimeOnly(10, 00),
            DurationMinutes = 60
        };

        var target = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Park",
            DayNumber = 1,
            Order = 2,
            PlannedTime = new TimeOnly(12, 00),
            DurationMinutes = 45
        };

        var tripPlaces = new List<TripPlace> { source, target };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, userId));

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(target.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(target);

        _tripPlaceRepository.Setup(x => x.GetByTripIdAndDayNumberAsync(tripId, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tripPlaces);

        var request = new ReorderPlacesRequest
        {
            TripId = tripId,
            SourceId = source.Id,
            TargetId = target.Id,
            DayNumber = 1
        };

        await CreateService().ReorderAsync(request, userId, CancellationToken.None);

        source.PlannedTime.Should().Be(new TimeOnly(12, 45));
        target.PlannedTime.Should().Be(new TimeOnly(12, 00));
    }

    [Fact]
    public async Task ReorderAsync_WhenMultiplePlacesOverlap_ShouldPushFollowingPlacesSequentially()
    {
        var userId = Guid.NewGuid();
        var tripId = Guid.NewGuid();

        var first = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Museum",
            DayNumber = 1,
            Order = 1,
            PlannedTime = new TimeOnly(9, 00),
            DurationMinutes = 90
        };

        var second = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Gallery",
            DayNumber = 1,
            Order = 2,
            PlannedTime = new TimeOnly(10, 30),
            DurationMinutes = 60
        };

        var third = new TripPlace
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            Name = "Park",
            DayNumber = 1,
            Order = 3,
            PlannedTime = new TimeOnly(11, 30),
            DurationMinutes = 45
        };

        var tripPlaces = new List<TripPlace> { first, second, third };

        _tripRepository.Setup(x => x.GetByIdAsync(tripId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateTrip(tripId, userId));

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(first.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(first);

        _tripPlaceRepository.Setup(x => x.GetByIdAsync(third.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(third);

        _tripPlaceRepository.Setup(x => x.GetByTripIdAndDayNumberAsync(tripId, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tripPlaces);

        var request = new ReorderPlacesRequest
        {
            TripId = tripId,
            SourceId = first.Id,
            TargetId = third.Id,
            DayNumber = 1
        };

        await CreateService().ReorderAsync(request, userId, CancellationToken.None);

        first.PlannedTime.Should().Be(new TimeOnly(12, 15));
        second.PlannedTime.Should().Be(new TimeOnly(10, 30));
        third.PlannedTime.Should().Be(new TimeOnly(11, 30));
    }

    private static Trip CreateTrip(Guid tripId, Guid userId)
    {
        return new Trip
        {
            Id = tripId,
            UserId = userId,
            Name = "Test trip",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(3),
            DestinationPlace = new Place { Id = Guid.NewGuid(), ExternalId = "destination", Name = "Paris" },
            TripShares = new List<TripShare>(),
            Places = new List<TripPlace>()
        };
    }
}