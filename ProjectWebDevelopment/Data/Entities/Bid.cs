using Microsoft.AspNetCore.Identity;

namespace ProjectWebDevelopment.Data.Entities
{
    public class Bid : Entity
    {
        public DateTime Date { get; set; }

        public double Price { get; set; }

        public int AuctionId { get; set; }

        public Auction? Auction { get; set; }
        
        public IdentityUser Buyer { get; set; }
    }
}
