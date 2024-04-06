using Microsoft.AspNetCore.Identity;

namespace ProjectWebDevelopment.Data.Entities
{
    public class AuctionUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<Auction>? Auctions { get; set; }

        public ICollection<Bid>? Bids { get; set; }
    }
}
