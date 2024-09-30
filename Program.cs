using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Storage.Blobs;      // Add the Azure Blob Storage namespace
using Azure.Storage.Files.Shares; // Add the Azure File Storage namespace
using Azure.Data.Tables;        // Add the Azure Table Storage namespace

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication() // Use ConfigureFunctionsWorkerDefaults to set up worker services
    .ConfigureServices(services =>
    {
        // Register Application Insights for telemetry
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Retrieve the Azure Storage connection string from environment variables
        string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        // Validate the connection string
        if (string.IsNullOrEmpty(storageConnectionString))
        {
            throw new InvalidOperationException("AzureWebJobsStorage connection string is not configured.");
        }

        // Register the ShareServiceClient for Azure File Storage
        services.AddSingleton(new ShareServiceClient(storageConnectionString));

        // Register the BlobServiceClient for Azure Blob Storage
        services.AddSingleton(new BlobServiceClient(storageConnectionString));

        // Register the TableServiceClient for Azure Table Storage
        services.AddSingleton(new TableServiceClient(storageConnectionString));
    })
    .Build();

host.Run();
