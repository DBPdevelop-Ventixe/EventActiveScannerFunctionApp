using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    // Azure keyvault to load secret database connection string
    .ConfigureAppConfiguration((context, config) =>
    {
        var builtConfig = config.Build();
        var keyVaultEndpoint = builtConfig["KeyVaultEndpoint"];
        if (!string.IsNullOrEmpty(keyVaultEndpoint))
        {
            config.AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential());
        }
    })

    // Configure the Functions Worker
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
