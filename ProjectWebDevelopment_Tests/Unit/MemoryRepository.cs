using ProjectWebDevelopment.Data.Entities;
using ProjectWebDevelopment.Services;

namespace ProjectWebDevelopment_Tests.Unit;

public class MemoryRepository : IAuctionRepository
{
    private readonly List<Auction> _auctions = new();

    private readonly List<Bid> _bids = new();

    public Task<IEnumerable<Auction>> GetAuctions()
    {
        return Task.FromResult<IEnumerable<Auction>>(_auctions);
    }

    public Task<Auction?> GetAuctionById(int id)
    {
        var auction = _auctions.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(auction);
    }

    public Task<int> CreateAuction(Auction auction, IEnumerable<string> imageSources)
    {
        var nextAutoIncrement = _auctions.Any() ? _auctions.Max(a => a.Id) + 1 : 1;
        auction.Id = nextAutoIncrement;
        _auctions.Add(auction);

        return Task.FromResult(nextAutoIncrement);
    }

    public async Task<int> UpdateAuction(Auction auction)
    {
        var indexOf = _auctions.FindIndex(a => a.Id == auction.Id);
        if (indexOf != -1)
        {
            _auctions[indexOf] = auction;
            return await Task.FromResult(1);
        }
        return await Task.FromResult(0);
    }

    public Task<int> DeleteAuction(int id)
    {
        var indexOf = _auctions.FindIndex(a => a.Id == id);
        if (indexOf == -1)
            return Task.FromResult(0);

        _auctions.RemoveAt(indexOf);
        return Task.FromResult(1);
    }

    public bool AuctionExists(int id)
    {
        return _auctions.Any(auction => auction.Id == id);
    }

    public Task<List<Bid>> GetBids(int auctionId)
    {
        var bids = _bids.Where(bid => bid.AuctionId == auctionId).ToList();
        return Task.FromResult(bids);
    }

    public Task<Bid?> GetBidById(int id)
    {
        var bid = _bids.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(bid);
    }

    public Task<int> CreateBid(Bid bid)
    {
        var nextAutoIncrement = _bids.Any() ? _bids.Max(b => b.Id) + 1 : 1;
        bid.Id = nextAutoIncrement;
        _bids.Add(bid);

        return Task.FromResult(nextAutoIncrement);
    }

    public Task<int> DeleteBid(int id)
    {
        var indexOf = _bids.FindIndex(bid => bid.Id == id);

        if (indexOf == -1)
            return Task.FromResult(0);

        _bids.RemoveAt(indexOf);
        return Task.FromResult(1);
    }

    public Task<IEnumerable<Auction>> GetAuctionsWithBids()
    {
        return Task.FromResult<IEnumerable<Auction>>(_auctions);
    }

    public Task<Auction?> GetAuctionByIdWithBids(int id)
    {
        return GetAuctionById(id);
    }
}