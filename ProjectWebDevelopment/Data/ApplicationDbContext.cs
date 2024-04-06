using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectWebDevelopment.Data.Entities;

namespace ProjectWebDevelopment.Data
{
    public class ApplicationDbContext : IdentityDbContext<AuctionUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Auction> Auctions { get; set; }

        public DbSet<Bid> Bids { get; set; }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Auction>(auction =>
            {
                auction.Property(auction => auction.Title).IsRequired().HasMaxLength(100);
                auction.Property(auction => auction.Description).IsRequired().HasMaxLength(500);
            });

            modelBuilder.Entity<Auction>()
                .HasOne(auction => auction.Seller)
                .WithMany(auctionUser => auctionUser.Auctions)
                .HasForeignKey(auction => auction.SellerId);

            modelBuilder.Entity<Bid>()
                .HasOne(bid => bid.Auction)
                .WithMany(auction => auction.Bids)
                .HasForeignKey(bid => bid.AuctionId);

            modelBuilder.Entity<Bid>()
                .HasOne(bid => bid.Buyer)
                .WithMany(buyer => buyer.Bids)
                .HasForeignKey(bid => bid.BuyerId);

            modelBuilder.Entity<Image>()
                .HasOne(image => image.Auction)
                .WithMany(auctionItem => auctionItem.Images)
                .HasForeignKey(image => image.AuctionId);

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Seller", NormalizedName = "SELLER" },
                new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Buyer", NormalizedName = "BUYER" }
            );
        }
    }
}
