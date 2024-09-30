using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AzureCloudFunctions
{
    public class ProductsFunction
    {
        private readonly TableServiceClient _tableServiceClient; // Table service client
        private readonly string _tableName = "Products"; // Replace with your actual table name

        // Constructor to inject TableServiceClient
        public ProductsFunction(TableServiceClient tableServiceClient)
        {
            _tableServiceClient = tableServiceClient;
        }

        // Function to get all products
        [Function("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            FunctionContext context)
        {
            var log = context.GetLogger<ProductsFunction>();
            log.LogInformation("Fetching all products from Table Storage.");

            var products = new List<Product>();

            try
            {
                // Get the table client for your specific table
                var tableClient = _tableServiceClient.GetTableClient(_tableName);

                // Query for all products
                await foreach (var product in tableClient.QueryAsync<Product>())
                {
                    products.Add(product);
                }

                return new OkObjectResult(products);
            }
            catch (Exception ex)
            {
                log.LogError($"Error fetching products: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
