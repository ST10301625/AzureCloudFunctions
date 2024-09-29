using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.Shares;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;

public class FileUploadFunction
{
    private readonly ShareServiceClient _shareServiceClient;  // Injected File Share service client

    // Constructor to inject ShareServiceClient
    public FileUploadFunction(ShareServiceClient shareServiceClient)
    {
        _shareServiceClient = shareServiceClient;
    }

    // Function to handle file uploads
    [Function("UploadFile")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        FunctionContext context) // Use FunctionContext to get logger
    {
        var log = context.GetLogger<FileUploadFunction>();
        log.LogInformation("Received a file upload request.");

        IFormFile file = req.Form.Files["file"];
        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("Please select a file to upload.");
        }

        string shareName = "fileshare";  // File share name
        string directoryName = "uploads";  // Directory name

        try
        {
            // Use the injected ShareServiceClient to interact with Azure File Storage
            ShareClient shareClient = _shareServiceClient.GetShareClient(shareName);
            ShareDirectoryClient directoryClient = shareClient.GetDirectoryClient(directoryName);
            ShareFileClient fileClient = directoryClient.GetFileClient(file.FileName);

            // Ensure the directory exists
            await directoryClient.CreateIfNotExistsAsync();

            using (var stream = file.OpenReadStream())
            {
                await fileClient.CreateAsync(stream.Length);  // Create file
                await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);  // Upload file content
            }

            return new OkObjectResult($"File '{file.FileName}' uploaded successfully.");
        }
        catch (Exception ex)
        {
            log.LogError($"File upload failed: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
