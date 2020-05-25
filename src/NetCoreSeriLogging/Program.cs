using System;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace NetCoreSeriLogging
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            CreateWebHostBuilder(args, cancellationTokenSource).Build().Run();
            cancellationTokenSource.Cancel();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args,
        CancellationTokenSource cancellationTokenSource) =>
            WebHost.CreateDefaultBuilder(args)
                                .UseStartup<Startup>()
                .ConfigureLogging((_, config) => config.ClearProviders())
                .UseSerilog((_, configuration) =>
                {
                    configuration
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Application", "NetCoreSeriLogging")
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                        {
                            CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                            AutoRegisterTemplate = true,
                            TemplateName = "serilog-events-template",
                            IndexFormat = "log-{0:yyyy.MM.dd}"
                        });
                });
    }
}
