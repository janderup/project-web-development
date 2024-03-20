using Microsoft.EntityFrameworkCore;
using ProjectWebDevelopment.Data;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Services
{
    public class AuctionService
    {
        private IAuctionRepository Repository { get; }

        private IAuctionImageProcessor ImageProcessor { get; }

        public AuctionService(
            IAuctionRepository repository,
            IAuctionImageProcessor imageProcessor
            )
        {
            Repository = repository;
            ImageProcessor = imageProcessor;
        }

        public async Task<IEnumerable<Auction>> GetAuctions()
        {
            return await Repository.GetAuctions();
        }

        public async Task<Auction?> GetAuctionById(int id)
        {
            return await Repository.GetAuctionById(id);
        }

        public async Task CreateAuction(Auction auction, IEnumerable<IFormFile> images)
        {
            // Validate the maximum length of an auction title
            if (auction.Title.Length > AuctionServiceSettings.MaxTitleLength)
                throw new InvalidOperationException(
                    $"The title of an auction may not be greater than {AuctionServiceSettings.MaxTitleLength} characters.");

            // Validate the maximum length of an auction description
            if (auction.Description.Length > AuctionServiceSettings.MaxDescriptionLength)
                throw new InvalidOperationException(
                    $"The description of an auction may not be greater than {AuctionServiceSettings.MaxDescriptionLength} characters.");

            // Validate the minimum bid when filled in
            if (auction.MinimumBid != null 
                && (auction.MinimumBid < AuctionServiceSettings.MinMinimumBid || auction.MinimumBid > AuctionServiceSettings.MaxMinimumBid))
                throw new InvalidOperationException(
                    $"The minimum bid of an auction must be between &euro; {AuctionServiceSettings.MinMinimumBid} and &euro; {AuctionServiceSettings.MaxMinimumBid}.");
            
            var imageList = images.ToList();
            
            // Check if the number of images uploaded is valid.
            if (imageList.Count < AuctionServiceSettings.MinImageCountPerAuction || imageList.Count > AuctionServiceSettings.MaxImageCountPerAuction)
                throw new InvalidOperationException(
                    $"You must upload between {AuctionServiceSettings.MinImageCountPerAuction} and {AuctionServiceSettings.MaxImageCountPerAuction} images per auction.");
    
            foreach (var imageFile in imageList)
            {
                // Length is measured in bytes. 1024x1024 represents the size in Megabytes.
                if (imageFile.Length > AuctionServiceSettings.MaxMegabytesPerImage * 1024 * 1024)
                    throw new InvalidOperationException("Image size must be less than 2 megabytes.");

                // Check if the file extensions are supported
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!AuctionServiceSettings.AllowedFileExtensions.Contains(extension))
                    throw new InvalidOperationException("The image file extension is not supported.");
            }

            var imageSources = imageList.Select(imageSource => ImageProcessor.ProcessUploadedImage(imageSource));
            await Repository.CreateAuction(auction, imageSources);
        }

        public async Task UpdateAuction(Auction auction)
        {
            await Repository.UpdateAuction(auction);
        }

        public async Task DeleteAuction(Auction auction)
        {
            if (auction.Bids != null && auction.Bids.Any())
                throw new InvalidOperationException("You may not delete an auction that has bids.");
            
            await Repository.DeleteAuction(auction.Id);
        }

        public bool AuctionExists(int id)
        {
            return Repository.AuctionExists(id);
        }

        public async Task PlaceBid(Bid bid)
        {
            if (bid.Price < 0)
                throw new InvalidOperationException("The bid may not be lower than &euro; 0.");

            var auction = await Repository.GetAuctionById(bid.AuctionId) 
                ?? throw new InvalidOperationException("The auction ID is invalid. The auction does not exist.");

            if (bid.Price < auction.MinimumBid)
                throw new InvalidOperationException("The bid may not be lower than the minimum bid of the auction.");

            var bids = await Repository.GetBids(bid.AuctionId);

            if (bids.Any())
            {
                var highestBidPrice = bids.Max(b => b.Price);
                if (bid.Price <= highestBidPrice)
                    throw new InvalidOperationException("The bid must be higher than the current highest bid.");
            }

            await Repository.CreateBid(bid);
        }
    }
}
