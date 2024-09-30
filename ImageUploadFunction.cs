using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs; // Assuming you're using Blob Storage for images
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AzureCloudFunctions
{
    public class ImageUploadFunction
    {
        private readonly BlobServiceClient _blobServiceClient; // Injected Blob service client

        // Constructor to inject BlobServiceClient
        public ImageUploadFunction(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        // Function to handle image uploads
        [Function("UploadImage")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            FunctionContext context)
        {
            var log = context.GetLogger<ImageUploadFunction>();
            log.LogInformation("Received an image upload request.");

            IFormFile file = req.Form.Files["file"];
            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("Please select a file to upload.");
            }

            try
            {
                string containerName = "products"; // Set your blob container name
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                // Ensure the container exists
                await containerClient.CreateIfNotExistsAsync();

                string blobName = file.FileName; // Customize the blob name if necessary
                var blobClient = containerClient.GetBlobClient(blobName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true); // Upload the image
                }

                string imageUrl = blobClient.Uri.ToString(); // Get the URL of the uploaded image
                log.LogInformation($"Image uploaded successfully: {imageUrl}");

                return new OkObjectResult(imageUrl); // Return the image URL
            }
            catch (Exception ex)
            {
                log.LogError($"Image upload failed: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
