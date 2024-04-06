using Microsoft.AspNetCore.SignalR;

namespace ProjectWebDevelopment.Services;

public class AuctionHub : Hub
{
    public async Task SendBidUpdate(int auctionId, string message)
    {
        await Clients.Group($"auction-{auctionId}").SendAsync("ReceiveBidUpdate", message);
    }

    public async Task JoinAuctionGroup(int auctionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"auction-{auctionId}");
    }

    public async Task LeaveAuctionGroup(int auctionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"auction-{auctionId}");
    }
}