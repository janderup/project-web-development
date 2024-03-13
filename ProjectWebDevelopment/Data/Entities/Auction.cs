using Microsoft.AspNetCore.Identity;

namespace ProjectWebDevelopment.Data.Entities
{
    public class Auction : Entity
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double? MinimumBid { get; set; }

        public AuctionItem? AuctionItem { get; set; }

        public int AuctionItemId { get; set; }

        public ICollection<Bid>? Bids { get; set; }

        public IdentityUser Seller { get; set; }

    }
}
