using Microsoft.AspNetCore.Identity;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Models
{
    public class AuctionDetailsViewModel
    {
        public Auction Auction { get; set; }

        public SignInManager<AuctionUser> SignInManager { get; set; }

        public AuctionDetailsViewModel(Auction auction, SignInManager<AuctionUser> signInManager)
        {
            this.Auction = auction;
            this.SignInManager = signInManager;
        }
    }
}
