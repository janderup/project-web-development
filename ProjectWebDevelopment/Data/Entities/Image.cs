namespace ProjectWebDevelopment.Data.Entities
{
    public class Image : Entity
    {
        public string Path { get; set; }

        public AuctionItem? AuctionItem { get; set; }

        public int AuctionItemId { get; set; }
    }
}
