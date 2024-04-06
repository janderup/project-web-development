using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectWebDevelopment.Data;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Services
{
    public class AuctionService
    {
        private IAuctionRepository Repository { get; }

        private IAuctionImageProcessor ImageProcessor { get; }

        private IAuctionMailer AuctionMailer { get; }

        private IHubContext<AuctionHub>? HubContext { get; }

        private UserManager<AuctionUser> UserManager { get; }

        public AuctionService(
            IAuctionRepository repository,
            IAuctionImageProcessor imageProcessor,
            IAuctionMailer auctionMailer,
            IHubContext<AuctionHub>? hubContext,
            UserManager<AuctionUser> userManager
            )
        {
            Repository = repository;
            ImageProcessor = imageProcessor;
            AuctionMailer = auctionMailer;
            HubContext = hubContext;
            UserManager = userManager;
        }

        public async Task<IEnumerable<Auction>> GetAuctions()
        {
            return await Repository.GetAuctions();
        }

        public async Task<IEnumerable<Auction>> GetAuctionsWithBids()
        {
            return await Repository.GetAuctionsWithBids();
        }

        public async Task<Auction?> GetAuctionById(int id)
        {
            return await Repository.GetAuctionById(id);
        }

        public async Task<Auction?> GetAuctionByIdWithBids(int id)
        {
            return await Repository.GetAuctionByIdWithBids(id);
        }

        public async Task<int> CreateAuction(Auction auction, IEnumerable<IFormFile> images)
        {
            // Validate the maximum length of an auction title
            if (auction.Title.Length > AuctionServiceSettings.MaxTitleLength)
                throw new InvalidOperationException(
                    $"De titel van de veiling mag niet groter zijn dan {AuctionServiceSettings.MaxTitleLength} karakters.");

            // Validate the maximum length of an auction description
            if (auction.Description.Length > AuctionServiceSettings.MaxDescriptionLength)
                throw new InvalidOperationException(
                    $"De beschrijving van de veiling mag niet groter zijn dan {AuctionServiceSettings.MaxDescriptionLength} karakters.");

            // Validate the minimum bid when filled in
            if (auction.MinimumBid != null 
                && (auction.MinimumBid < AuctionServiceSettings.MinMinimumBid || auction.MinimumBid > AuctionServiceSettings.MaxMinimumBid))
                throw new InvalidOperationException(
                    $"Het minimumbod van de veiling moet liggen tussen de &euro; {AuctionServiceSettings.MinMinimumBid} en &euro; {AuctionServiceSettings.MaxMinimumBid}.");
            
            var imageList = images.ToList();
            
            // Check if the number of images uploaded is valid.
            if (imageList.Count < AuctionServiceSettings.MinImageCountPerAuction || imageList.Count > AuctionServiceSettings.MaxImageCountPerAuction)
                throw new InvalidOperationException(
                    $"Je moet tussen de {AuctionServiceSettings.MinImageCountPerAuction} en {AuctionServiceSettings.MaxImageCountPerAuction} afbeeldingen gebruiken per veiling.");
    
            foreach (var imageFile in imageList)
            {
                // Length is measured in bytes. 1024x1024 represents the size in Megabytes.
                if (imageFile.Length > AuctionServiceSettings.MaxMegabytesPerImage * 1024 * 1024)
                    throw new InvalidOperationException($"Elke afbeelding mag niet groter zijn dan {AuctionServiceSettings.MaxMegabytesPerImage} megabytes.");

                // Check if the file extensions are supported
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!AuctionServiceSettings.AllowedFileExtensions.Contains(extension))
                    throw new InvalidOperationException("Het type bestand van één of meerdere afbeeldingen wordt niet ondersteund.");
            }

            var imageSources = imageList.Select(imageSource => ImageProcessor.ProcessUploadedImage(imageSource));
            return await Repository.CreateAuction(auction, imageSources);
        }

        public async Task UpdateAuction(Auction auction)
        {
            await Repository.UpdateAuction(auction);
        }

        public async Task DeleteAuction(Auction auction)
        {
            if (auction.Bids != null && auction.Bids.Any())
                throw new InvalidOperationException("Je mag geen veiling annuleren die al biedingen heeft.");
            
            await Repository.DeleteAuction(auction.Id);
        }

        public bool CanAuctionBeCancelled(Auction auction)
        {
            if (auction.HasEnded())
                return false;

            if (auction.Bids != null && auction.Bids.Any())
                return false;

            return true;
        }

        public bool AuctionExists(int id)
        {
            return Repository.AuctionExists(id);
        }

        public async Task<IEnumerable<Bid>> GetBids(int auctionId)
        {
            return await Repository.GetBids(auctionId);
        }

        public async Task PlaceBid(Bid bid)
        {
            if (bid.Price <= 0)
                throw new InvalidOperationException("Het bod mag niet gelijk zijn of kleiner dan &euro; 0.");

            var auction = await Repository.GetAuctionById(bid.AuctionId)
                ?? throw new InvalidOperationException("Het veiling ID is ongeldig. De veiling bestaat niet.");

            if (bid.Price < auction.MinimumBid)
                throw new InvalidOperationException("Het bod mag niet lager zijn dan het minimumbod van de veiling.");

            if (auction.HasEnded())
                throw new InvalidOperationException("Je mag geen bieding plaatsen op een veiling die al is afgelopen.");

            var bids = await Repository.GetBids(bid.AuctionId);

            var highestBid = bids.OrderByDescending(b => b.Price).FirstOrDefault();

            if (highestBid != null && bid.Price <= highestBid.Price)
                throw new InvalidOperationException("Het bod moet hoger zijn dan het huidige hoogste bod."); ;

            
            await Repository.CreateBid(bid);

            
            if (HubContext != null)
                await NotifyAuctionHub(bid);

            if (highestBid != null)
                AuctionMailer.SendOutbidNotification(auction, highestBid, bid);
        }

        // Returns the next minimum bid that is accepted for this auction.
        // Note: auction.Bids must be initialized before using this method.
        public double GetNextMinimumBid(Auction auction)
        {
            if (auction.Bids == null)
                throw new ArgumentException("Auction bids are null. Bids must be initialized before determaining the next minimum bid.");

            if (!auction.Bids.Any())
                return auction.MinimumBid ?? 0.01;

            var highestBid = auction.Bids.Max(bid => bid.Price);
            return Math.Round(highestBid + 0.01, 2);
        }

        public async Task NotifyAuctionHub(Bid bid)
        {
            // The buyer is not included at this point, so we must retrieve this from the user manager.
            var buyer = await UserManager.FindByIdAsync(bid.BuyerId);

            Dictionary<string, object> bidData = new()
            {
                { "Name", buyer?.FirstName + " " + buyer?.LastName },
                { "Price", bid.Price },
                { "Date", bid.Date },
                { "NextMinimum", Math.Round(bid.Price + 0.01, 2) }
            };

            string json = JsonConvert.SerializeObject(bidData, Formatting.Indented);

            // Send the JSON string to the client
            await HubContext.Clients.Group($"auction-{bid.AuctionId}")
                .SendAsync("ReceiveBidUpdate", json);
        }
    }
}
