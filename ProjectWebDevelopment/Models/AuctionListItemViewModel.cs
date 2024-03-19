namespace ProjectWebDevelopment.Models
{
    public class AuctionListItemViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public double? HighestBid { get; set; }

        public DateTime EndDate { get; set; }

    }
}
