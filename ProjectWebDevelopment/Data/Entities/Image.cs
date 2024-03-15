namespace ProjectWebDevelopment.Data.Entities
{
    public class Image : Entity
    {
        public string Path { get; set; }

        public Auction? Auction { get; set; }

        public int AuctionId { get; set; }
    }
}
