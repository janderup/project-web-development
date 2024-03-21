using Microsoft.EntityFrameworkCore;
using ProjectWebDevelopment.Data;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Services
{
    public class AuctionRepositoryEF : IAuctionRepository
    {
        private readonly ApplicationDbContext _context;

        public AuctionRepositoryEF(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAuction(Auction auction, IEnumerable<string> imageSources)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.AddAsync(auction);
                await _context.SaveChangesAsync();
                
                foreach (var imageSource in imageSources)
                {
                    Image auctionImage = new()
                    {
                        Path = imageSource,
                        AuctionId = auction.Id
                    };
                    _context.Add(auctionImage);
                }

                var rowsAffected = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return rowsAffected;

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> CreateBid(Bid bid)
        {
            var insertedBid = await _context.Bids.AddAsync(bid);
            await _context.SaveChangesAsync();
            return insertedBid.Entity.Id;
        }

        public async Task<int> DeleteAuction(int id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if (auction == null)
                return 0;

            _context.Auctions.Remove(auction);
            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }

        public bool AuctionExists(int id)
        {
            return _context.Auctions.Any(e => e.Id == id);
        }

        public async Task<int> DeleteBid(int id)
        {
            var bid = await _context.Bids.FindAsync(id);

            if (bid == null) 
                return 0;

            _context.Bids.Remove(bid);
            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected;
        }
        
        public async Task<Auction?> GetAuctionById(int id)
        {
            return await _context.Auctions
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Auction>> GetAuctions()
        {
            return await _context.Auctions.Include(a => a.Images).ToListAsync();
        }

        public async Task<Bid?> GetBidById(int id)
        {
            return await _context.Bids.FindAsync(id);
        }

        public async Task<List<Bid>> GetBids(int auctionId)
        {
            return await _context.Bids
                .Include(bid => bid.Buyer)
                .Where(bid => bid.AuctionId == auctionId)
                .OrderByDescending(b => b.Price)
                .ToListAsync();
        }

        public async Task<int> UpdateAuction(Auction auction)
        {
            _context.Auctions.Update(auction);
            return await _context.SaveChangesAsync();
        }
    }
}
