using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Auction> Auctions { get; set; }

        public DbSet<AuctionItem> AuctionItems { get; set; }

        public DbSet<Bid> Bids { get; set; }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuctionItem>(auctionItem =>
            {
                auctionItem.Property(auctionItem => auctionItem.Title).IsRequired().HasMaxLength(100);
                auctionItem.Property(auctionItem => auctionItem.Description).IsRequired().HasMaxLength(500);
            });

            modelBuilder.Entity<Auction>()
                .HasOne(auction => auction.AuctionItem)
                .WithOne(auctionItem => auctionItem.Auction)
                .HasForeignKey<Auction>(auction => auction.AuctionItemId)
                .IsRequired();

            modelBuilder.Entity<Bid>()
                .HasOne(bid => bid.Auction)
                .WithMany(auction => auction.Bids)
                .HasForeignKey(bid => bid.AuctionId);

            modelBuilder.Entity<Image>()
                .HasOne(image => image.AuctionItem)
                .WithMany(auctionItem => auctionItem.Images)
                .HasForeignKey(image => image.AuctionItemId);
        }
    }
}
