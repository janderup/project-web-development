using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectWebDevelopment.Data;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Models
{
    public class CreateAuctionViewModel
    {
        public List<SelectListItem> AuctionDurationOptions { get; set; }

        public AuctionDuration AuctionDuration { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ICollection<IFormFile>? Images { get; set; }

        public double? MinimumBid { get; set; }

        public CreateAuctionViewModel()
        {
            AuctionDurationOptions = Enum.GetValues(typeof(AuctionDuration))
                .Cast<AuctionDuration>()
                .Select(auctionDuration => new SelectListItem
                {
                    Value = ((int) auctionDuration).ToString(),
                    Text = auctionDuration.GetDescription()
                })
                .ToList();
        }
    }
}
