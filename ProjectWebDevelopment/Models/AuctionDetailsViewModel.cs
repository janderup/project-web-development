using Microsoft.AspNetCore.Identity;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Models
{
    public class AuctionDetailsViewModel
    {
        public Auction Auction { get; set; }

        public SignInManager<AuctionUser> SignInManager { get; set; }

        public bool CanAuctionBeCancelled { get; set; }

        public double NextMinimumBid { get; set; }

        public AuctionDetailsViewModel(
            Auction auction, SignInManager<AuctionUser> signInManager, 
            bool canAuctionBeCancelled, 
            double nextMinimumBid
            )
        {
            this.Auction = auction;
            this.SignInManager = signInManager;
            this.CanAuctionBeCancelled = canAuctionBeCancelled;
            this.NextMinimumBid = nextMinimumBid;
        }
    }
}
