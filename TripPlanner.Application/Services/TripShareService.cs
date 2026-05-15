using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Exceptions;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Interfaces.Services;
using TripPlanner.Application.Interfaces.Utilities;
using TripPlanner.Domain.Entities;

namespace TripPlanner.Application.Services
{
    public class TripShareService : ITripShareService
    {
        private readonly ITripShareRepository _tripShareRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TripShareService(ITripShareRepository tripShareRepository, ITripRepository tripRepository, IUnitOfWork unitOfWork)
        {
            _tripShareRepository = tripShareRepository;
            _tripRepository = tripRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(Guid tripId, AddTripShareRequest request, Guid userId, CancellationToken cancellationToken)
        {
            await CheckAccessAsync(tripId, userId, cancellationToken);

            await _tripShareRepository.AddAsync(new TripShare
            {
                TripId = tripId,
                UserId = request.UserId,
                Permission = request.Permission,
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<TripShareResponse>> GetByTripIdAsync(Guid tripId, Guid userId, CancellationToken cancellationToken)
        {
            await CheckAccessAsync(tripId, userId, cancellationToken);

            return (await _tripShareRepository.GetByTripIdAsync(tripId, cancellationToken)).Select(t => new TripShareResponse
            {
                Id = t.Id,
                UserId = t.UserId,
                UserFullName = $"{t.User.FirstName} {t.User.LastName}",
                Permission = t.Permission
            });
        }

        public async Task RemoveAsync(Guid tripId, Guid tripShareId, Guid userId, CancellationToken cancellationToken)
        {
            await CheckAccessAsync(tripId, userId, cancellationToken);

            await _tripShareRepository.DeleteAsync(tripShareId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Guid tripId, Guid tripShareId, UpdateTripShareRequest request, Guid userId, CancellationToken cancellationToken)
        {
            await CheckAccessAsync(tripId, userId, cancellationToken);

            var tripShareToUpdate = await _tripShareRepository.GetByIdAsync(tripShareId, cancellationToken);
            tripShareToUpdate?.Permission = request.Permission;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task CheckAccessAsync(Guid tripId, Guid userId, CancellationToken cancellationToken)
        {
            Trip? trip = await _tripRepository.GetByIdAsync(tripId, cancellationToken);
            if (trip == null)
            {
                throw new NotFoundException("Trip not found.");
            }

            if (trip.UserId != userId)
            {
                throw new ForbiddenException("Access denied.");
            }
        }
    }
}
