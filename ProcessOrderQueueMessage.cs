using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class ProcessOrderQueueMessage
{
    [FunctionName("ProcessOrderQueueMessage")]
    public async Task Run(
        [QueueTrigger("orders-queue", Connection = "AzureWebJobsStorage")] string queueMessage,
        ILogger log)
    {
        // Log the incoming message
        log.LogInformation($"Received message from queue: {queueMessage}");

        // Process the message: In this example, we're just logging it,
        // but you could extend this by saving it to a database or sending it elsewhere.

        // Simulate some asynchronous work if needed
        await Task.CompletedTask;

        // Log completion of the process
        log.LogInformation("Queue message processed successfully.");
    }
}
