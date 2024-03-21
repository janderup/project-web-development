using Microsoft.AspNetCore.Identity;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Models
{
    public class AuctionDetailsViewModel
    {
        public Auction Auction { get; set; }
        
        public IEnumerable<Bid> Bids { get; set; }

        public SignInManager<AuctionUser> SignInManager { get; set; }

        public AuctionDetailsViewModel(Auction auction, IEnumerable<Bid> bids, SignInManager<AuctionUser> signInManager)
        {
            this.Auction = auction;
            this.Bids = bids;
            this.SignInManager = signInManager;
        }
    }
}
