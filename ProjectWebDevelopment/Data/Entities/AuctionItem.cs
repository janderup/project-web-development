namespace ProjectWebDevelopment.Data.Entities
{
    public class AuctionItem : Entity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Auction Auction { get; set; }

        public ICollection<Image> Images { get; set; }


    }
}
