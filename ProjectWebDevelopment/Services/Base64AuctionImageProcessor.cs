
using System.IO;

namespace ProjectWebDevelopment.Services
{
    public class Base64AuctionImageProcessor : IAuctionImageProcessor
    {
        public string ProcessUploadedImage(IFormFile formFile)
        {
            var mimeType = formFile.ContentType;
            var base64 = ConvertImageToBase64(formFile);

            return $"data:{mimeType};base64,{base64}";
        }

        private string ConvertImageToBase64(IFormFile image)
        {
            using var memoryStream = new MemoryStream();

            // Kopieer de gegevens van de afbeelding naar het geheugenstroom
            image.CopyTo(memoryStream);

            // Converteer het geheugenstroom naar een byte array
            byte[] imageBytes = memoryStream.ToArray();

            // Converteer de byte array naar een base64-string
            string base64String = Convert.ToBase64String(imageBytes);

            return base64String;
        }
    }
}
