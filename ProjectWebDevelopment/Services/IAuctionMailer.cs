using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Services
{
    public interface IAuctionMailer
    {
        void SendOutbidNotification(Auction auction, Bid highestBid, Bid newHighestBid);
    }
}
