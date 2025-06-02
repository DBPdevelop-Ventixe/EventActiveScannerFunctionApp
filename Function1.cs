using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebFunctionApp
{
    public class Function1
    {
        public Function1(ILogger<Function1> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("SqlDbConnection")!;
            _logger = logger;
        }

        /*
         *  Got some help from GROK to figure this query out. And how to execute it.
         */

        private readonly IConfiguration _configuration;
        private readonly ILogger<Function1> _logger;
        private string connectionString = "";
        private string query =
            @"
                UPDATE Events
                SET Status = 'Past'
                WHERE Status = 'Active'
                    AND EventDate < @TodayDate;
            ";

        [Function("Function1")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            using (var connection = new SqlConnection(connectionString))
            {
                // Open connection to db
                connection.Open();

                // Do the thing
                using (var command = new SqlCommand(query, connection))
                {
                    // Add parameter for today's date
                    command.Parameters.AddWithValue("@TodayDate", DateTime.Today);

                    // Execute the command and log the result
                    int rowsAffected = command.ExecuteNonQuery();
                    _logger.LogInformation($"Event statuses changed: {rowsAffected}");
                }
            }

            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}