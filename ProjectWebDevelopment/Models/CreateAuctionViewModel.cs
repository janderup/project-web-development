using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Models
{
    public class CreateAuctionViewModel
    {

        public DateTime EndDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ICollection<IFormFile>? Images { get; set; }

        public double? MinimumBid { get; set; }

    }
}
