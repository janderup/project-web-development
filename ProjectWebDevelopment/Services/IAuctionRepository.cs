using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Services
{
    public interface IAuctionRepository
    {
        Task<IEnumerable<Auction>> GetAuctions();

        Task<IEnumerable<Auction>> GetAuctionsWithBids();

        Task<Auction?> GetAuctionById(int id);

        Task<Auction?> GetAuctionByIdWithBids(int id);

        Task<int> CreateAuction(Auction auction, IEnumerable<string> imageSources);

        Task<int> UpdateAuction(Auction auction);

        Task<int> DeleteAuction(int id);

        bool AuctionExists(int id);

        Task<List<Bid>> GetBids(int auctionId);

        Task<Bid?> GetBidById(int id);

        Task<int> CreateBid(Bid bid);

        Task<int> DeleteBid(int id);
    }
}
