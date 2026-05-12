using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Models;

namespace TripPlanner.Application.Services
{
    public class PlaceDetailsGenerationProcessor : IPlaceDetailsGenerationProcessor
    {
        private readonly IPlaceDetailsRepository _placeDetailsRepository;
        private readonly IImageProvider _imageProvider;
        private readonly IDescriptionProvider _descriptionProvider;
        private readonly IUnitOfWork _unitOfWork;

        public PlaceDetailsGenerationProcessor(
            IPlaceDetailsRepository placeDetailsRepository,
            IImageProvider imageProvider,
            IDescriptionProvider descriptionProvider,
            IUnitOfWork unitOfWork)
        {
            _placeDetailsRepository = placeDetailsRepository;
            _imageProvider = imageProvider;
            _descriptionProvider = descriptionProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task ProcessAsync(
            PlaceDetailsGenerationJob job,
            CancellationToken cancellationToken)
        {
            var placeDetails = await _placeDetailsRepository
                .GetByIdAsync(job.Id, cancellationToken);
            if (placeDetails == null)
            {
                return;
            }

            bool hasChanges = false;

            try
            {
                //if (string.IsNullOrEmpty(placeDetails.Description))
                //{
                //    placeDetails.Description = await _descriptionProvider.GetDescriptionAsync(job.PlaceName, job.PlaceLocation, cancellationToken);
                //    hasChanges = true;
                //}

                if (string.IsNullOrEmpty(placeDetails.ImageUrl))
                {
                    placeDetails.ImageUrl = await _imageProvider.GetImageUrlAsync($"{job.PlaceName}, {job.PlaceLocation}", cancellationToken);
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
