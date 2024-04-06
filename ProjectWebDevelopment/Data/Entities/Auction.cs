using Microsoft.AspNetCore.Identity;

namespace ProjectWebDevelopment.Data.Entities
{
    public class Auction : Entity
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ICollection<Image>? Images { get; set; }

        public double? MinimumBid { get; set; }

        public ICollection<Bid>? Bids { get; set; }

        public string SellerId { get; set; }

        public AuctionUser? Seller { get; set; }

        public bool HasEnded()
        {
            return DateTime.Now > EndDate;
        }
    }
}
