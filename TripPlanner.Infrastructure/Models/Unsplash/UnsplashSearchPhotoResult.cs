namespace TripPlanner.Infrastructure.Models.Unsplash
{
    public class UnsplashSearchPhotoResult
    {
        public List<UnsplashSearchPhoto> Results { get; set; }
    }

    public class UnsplashSearchPhoto
    {
        public UnsplashSearchPhotoUrls Urls { get; set; }
    }

    public class UnsplashSearchPhotoUrls
    {
        public string Regular { get; set; }
    }
}
