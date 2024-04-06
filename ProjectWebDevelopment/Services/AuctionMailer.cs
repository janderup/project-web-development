
using System.Net.Mail;
using System.Net;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Services
{
    public class AuctionMailer : IAuctionMailer
    {
        private readonly string SmtpAddress;

        private readonly string SmtpUsername;

        private readonly string SmtpPassword;

        public AuctionMailer(string smtpAddress, string smtpUsername, string smtpPassword) { 
            SmtpAddress = smtpAddress;
            SmtpUsername = smtpUsername;
            SmtpPassword = smtpPassword;
        }

        public void SendOutbidNotification(Auction auction, Bid highestBid, Bid newHighestBid)
        {
            var client = new SmtpClient(SmtpAddress, 2525)
            {
                Credentials = new NetworkCredential(SmtpUsername, SmtpPassword),
                EnableSsl = true
            };

            string message = $"Je bent overboden door {highestBid.Buyer.FirstName} met &euro; {highestBid.Price} op het product: {auction.Title}.";
            client.Send("veilingen@wiebiedt.hbo-ict.org", highestBid.Buyer.Email, "Je bent overboden!", message);
        }
    }
}
