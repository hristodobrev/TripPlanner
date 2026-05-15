using TripPlanner.Application.Interfaces.Providers;
using TripPlanner.Application.Interfaces.Repositories;
using TripPlanner.Application.Interfaces.Utilities;
using TripPlanner.Application.Models;

namespace TripPlanner.Application.Services
{
    public class PlaceDetailsGenerationProcessor : IPlaceDetailsGenerationProcessor
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IImageProvider _imageProvider;
        private readonly IDescriptionProvider _descriptionProvider;
        private readonly IUnitOfWork _unitOfWork;

        public PlaceDetailsGenerationProcessor(
            IPlaceRepository placeRepository,
            IImageProvider imageProvider,
            IDescriptionProvider descriptionProvider,
            IUnitOfWork unitOfWork)
        {
            _placeRepository = placeRepository;
            _imageProvider = imageProvider;
            _descriptionProvider = descriptionProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task ProcessAsync(
            PlaceDetailsGenerationJob job,
            CancellationToken cancellationToken)
        {
            var place = await _placeRepository
                .GetByIdAsync(job.Id, cancellationToken);
            if (place == null)
            {
                return;
            }

            bool hasChanges = false;

            try
            {
                if (string.IsNullOrEmpty(place.Description))
                {
                    place.Description = await _descriptionProvider.GetDescriptionAsync(job.PlaceName, job.PlaceLocation, cancellationToken);
                    hasChanges = true;
                }

                if (string.IsNullOrEmpty(place.ImageUrl))
                {
                    place.ImageUrl = await _imageProvider.GetImageUrlAsync($"{job.PlaceName}, {job.PlaceLocation}", cancellationToken);
                    hasChanges = true;
                }
            }
            catch (Exception)
            {
                // TODO: Add logging when exception is thrown, but do not interrupt the application
            }

            if (hasChanges)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
