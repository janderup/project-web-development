using Microsoft.AspNetCore.Identity;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Models
{
    public class AuctionsListViewModel
    {
        public IEnumerable<AuctionListItemViewModel> Auctions { get; set; }

        public SignInManager<AuctionUser> SignInManager { get; set; }

        public AuctionsListViewModel(IEnumerable<AuctionListItemViewModel> auctions, SignInManager<AuctionUser> signInManager) { 
            this.Auctions = auctions;
            this.SignInManager = signInManager;
        }
    }
}
