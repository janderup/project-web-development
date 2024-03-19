using ProjectWebDevelopment.Data;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Services
{
    public class AuctionService
    {
        protected ApplicationDbContext _context;

        protected IAuctionImageProcessor _imageProcessor { get; set; }

        public AuctionService(
            ApplicationDbContext context,
            IAuctionImageProcessor imageProcessor
            )
        {
            _context = context;
            _imageProcessor = imageProcessor;
        }

        public async Task CreateAuction(Auction auction, IEnumerable<IFormFile>? images)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _context.AddAsync(auction);
                await _context.SaveChangesAsync();

                if (images != null)
                {
                    foreach (var image in images)
                    {
                        Image auctionImage = new()
                        {
                            Path = _imageProcessor.ProcessUploadedImage(image),
                            AuctionId = auction.Id
                        };
                        _context.Add(auctionImage);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

            } catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task PlaceBid(Bid bid)
        {
            await _context.AddAsync(bid);
        }
    }
}
