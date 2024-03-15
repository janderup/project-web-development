using Microsoft.AspNetCore.Identity;

namespace ProjectWebDevelopment.Data.Entities
{
    public class AuctionUser : IdentityUser
    {
        public ICollection<Auction>? Auctions { get; set; }

        public ICollection<Bid>? Bids { get; set; }
    }
}
