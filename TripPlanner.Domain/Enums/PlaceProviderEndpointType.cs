namespace TripPlanner.Domain.Enums
{
    public enum PlaceProviderEndpointType
    {
        // SKU: Autocomplete Requests -> $2.83 per 1000 requests (0.00283 per request)
        AutoCompleteRequests = 1,

        // SKU: Place Details Essentials (IDs Only)
        PlaceDetailsEssentialsIdOnly = 2,
        // SKU: Place Details Essentials -> $5.00 per 1000 requests (0.005 per request)
        PlaceDetailsEssentials = 3,
        // SKU: Place Details Pro -> $17.00 per 1000 requests (0.017 per request)
        PlaceDetailsPro = 4,
        // SKU: Place Details Enterprise -> $20.00 per 1000 requests (0.02 per request)
        PlaceDetailsEnterprise = 5,

        // SKU: Place Details Photos -> $7.00 per 1000 requests (0.007 per request)
        PlaceDetailsPhotos = 6,

        // SKU: Text Search Essentials (IDs Only)
        TextSearchEssentialsIdOnly = 7,
        // SKU: Text Search Pro -> $32.00 per 1000 requests (0.032 per request)
        TextSearchPro = 8,
        // SKU: Text Search Enterprise -> $35.00 per 1000 requests (0.035 per request)
        TextSearchEnterprise = 9,

        // SKU: Nearby Search Pro -> $32.00 per 1000 requests (0.032 per request)
        NearbySearchPro = 10,
        // SKU: Nearby Search Enterprise -> $35.00 per 1000 requests (0.035 per request)
        NearbySearchEnterprise = 11,
    }
}
