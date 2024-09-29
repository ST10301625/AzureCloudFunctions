using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Storage.Files.Shares; // Add the Azure File Storage namespace

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication() // Use ConfigureFunctionsWorkerDefaults to set up worker services
    .ConfigureServices(services =>
    {
        // Register Application Insights for telemetry
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Register the Azure File Share client service
        string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        // Validate the connection string
        if (string.IsNullOrEmpty(storageConnectionString))
        {
            throw new InvalidOperationException("AzureWebJobsStorage connection string is not configured.");
        }

        // This registers the ShareServiceClient as a singleton to be used in the function
        services.AddSingleton(new ShareServiceClient(storageConnectionString));
    })
    .Build();

host.Run();
