namespace ProjectWebDevelopment.Services
{
    public interface IAuctionImageProcessor
    {
        // Processes an image and returns the src of the image that can be used in the front-end of the website.
        string ProcessUploadedImage(IFormFile formFile);
    }
}
