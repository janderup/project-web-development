using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Moq;
using ProjectWebDevelopment.Data.Entities;
using ProjectWebDevelopment.Services;

namespace ProjectWebDevelopment_Tests;

public class Tests
{
    private MemoryRepository Repository { get; set; }
    
    private Mock<IAuctionImageProcessor> ImageProcessorMock { get; set;  }
    
    private Mock<IHubContext<AuctionHub>> HubContextMock { get; set;  }
    
    private Mock<UserManager<AuctionUser>> UserManagerMock { get; set;  }

    private AuctionService AuctionService { get; set; }
    
    [SetUp]
    public void Setup()
    {
        Repository = new MemoryRepository();
        ImageProcessorMock = new Mock<IAuctionImageProcessor>();
        ImageProcessorMock
            .Setup(processor => processor.ProcessUploadedImage(It.IsAny<IFormFile>()))
            .Returns((IFormFile file) => file.FileName);
        HubContextMock = new Mock<IHubContext<AuctionHub>>();
        // UserManagerMock = new Mock<UserManager<AuctionUser>>();
        AuctionService = new AuctionService(Repository, ImageProcessorMock.Object, HubContextMock.Object, null);
    }

    [Test]
    public void CreateAuction_TooLongTitle_ThrowsException()
    {
        // Builds a string that is exactly 1 character longer than the max length.
        var title = new string('A', AuctionServiceSettings.MaxTitleLength + 1);
        var auction = ScaffoldAuction(title, "Lorem ipsum sit dolor amet", null);
    
        Assert.That(async () => await AuctionService.CreateAuction(auction, new List<IFormFile>()), Throws.InvalidOperationException);
    }

    [Test]
    public void CreateAuction_TooLongDescription_ThrowsException()
    {
        // Builds a string that is exactly 1 character longer than the max length.
        var description = new string('A', AuctionServiceSettings.MaxDescriptionLength + 1);
        var auction = ScaffoldAuction("Lorem ipsum sit dolor amet", description, null);
    
        Assert.That(async () => await AuctionService.CreateAuction(auction, new List<IFormFile>()), Throws.InvalidOperationException);
    }

    [TestCase(AuctionServiceSettings.MinMinimumBid - 1)]
    [TestCase(AuctionServiceSettings.MaxMinimumBid + 1)]
    public void CreateAuction_InvalidMinimumBid_ThrowsException(double minimumBid)
    {
        var auction = ScaffoldAuction("Lorem ipsum sit dolor amet", "Lorem ipsum sit dolor amet", minimumBid);
        
        Assert.That(async () => await AuctionService.CreateAuction(auction, new List<IFormFile>()), Throws.InvalidOperationException);
    }

    [TestCase(AuctionServiceSettings.MinImageCountPerAuction - 1)]
    [TestCase(AuctionServiceSettings.MaxImageCountPerAuction + 1)]
    public void CreateAuction_InvalidImageCount_ThrowsException(int numberOfImages)
    {
        var auction = ScaffoldAuction("Lorem ipsum sit dolor amet", "Lorem ipsum sit dolor amet", null);

        List<IFormFile> images = new();

        for (var i = 0; i < numberOfImages; i++)
        {
            var newImage = new Mock<IFormFile>();
            images.Add(newImage.Object);
        }
        
        Assert.That(async () => await AuctionService.CreateAuction(auction, images), Throws.InvalidOperationException);
    }
    
    [Test]
    public void CreateAuction_TooLargeImageFile_ThrowsException()
    {
        var auction = ScaffoldAuction("Lorem ipsum sit dolor amet", "Lorem ipsum sit dolor amet", null);
        List<IFormFile> images = new();

        var tooLargeImage = new Mock<IFormFile>();
        tooLargeImage
            .Setup(image => image.Length)
            .Returns((AuctionServiceSettings.MaxMegabytesPerImage + 1) * 1024 * 1024);
        images.Add(tooLargeImage.Object);
        
        Assert.That(async () => await AuctionService.CreateAuction(auction, images), Throws.InvalidOperationException);
    }

    [TestCase("evil.exe")]
    [TestCase("MyVacation.mp4")]
    [TestCase("HelloWorld.txt")]
    public void CreateAuction_UnsupportedFileExtension_ThrowsException(string fileName)
    {
        var auction = ScaffoldAuction("Lorem ipsum sit dolor amet", "Lorem ipsum sit dolor amet", null);
        List<IFormFile> images = new();

        var invalidFile = new Mock<IFormFile>();
        invalidFile
            .Setup(image => image.Length)
            .Returns(10);

        invalidFile
            .Setup(image => image.FileName)
            .Returns(fileName);
        
        images.Add(invalidFile.Object);
        
        Assert.That(async () => await AuctionService.CreateAuction(auction, images), Throws.InvalidOperationException);
    }

    [Test]
    public void CreateAuction_ValidData_NoExceptionThrown()
    {
        var auction = ScaffoldAuction("Lorem ipsum sit dolor amet", "Lorem ipsum sit dolor amet", null);
        List<IFormFile> images = new();

        var validFile = new Mock<IFormFile>();
        validFile.Setup(image => image.Length).Returns(10);
        validFile.Setup(image => image.FileName).Returns("HelloWorld.jpg");

        images.Add(validFile.Object);
    
        // Assert that no exception is thrown
        Assert.DoesNotThrowAsync(async () => await AuctionService.CreateAuction(auction, images));
    }
    
    // Method used to quickly scaffold a new auction
    private static Auction ScaffoldAuction(string title, string description, double? minimumBid)
    {
        return new Auction()
        {
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1),
            Title = title,
            Description = description,
            Images = null,
            MinimumBid = minimumBid,
            Bids = null,
            SellerId = Guid.NewGuid().ToString()
        };
    }
}