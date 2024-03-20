namespace ProjectWebDevelopment.Services;

using System.Collections.Generic;

public static class AuctionServiceSettings
{
    // Maximum length of an auction title
    public const int MaxTitleLength = 100;

    // Maximum length of an auction description
    public const int MaxDescriptionLength = 500;

    // Min minimum bid that is accepted when filled in.
    public const int MinMinimumBid = 1;

    // Maximum minimum bid that is accepted when filled in.
    public const int MaxMinimumBid = 1000000;

    // Minimum image count of a single auction
    public const int MinImageCountPerAuction = 1;
    
    // Maximum number of images that a user can upload to a single auction.
    public const int MaxImageCountPerAuction = 5;

    // Maximum filesize per image that is uploaded to an auction.
    public const int MaxMegabytesPerImage = 5;

    // Allowed file extensions for the images.
    public static readonly HashSet<string> AllowedFileExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png" };
}
