<h1>Serilog.AspNetCore</h1>
<p>Serilog logging for ASP.NET Core. This package routes ASP.NET Core log messages through Serilog, so you can get information about ASP.NET's internal operations written to the same Serilog sinks as your application events.</p>
<p>With&nbsp;<em>Serilog.AspNetCore</em>&nbsp;installed and configured, you can write log messages directly through Serilog or any&nbsp;<code>ILogger</code>&nbsp;interface injected by ASP.NET. All loggers will use the same underlying implementation, levels, and destinations.</p>
<h3><a href="https://github.com/serilog/serilog-aspnetcore#instructions" aria-hidden="true"></a>Instructions</h3>
<p><strong>First</strong>, install the&nbsp;<em>following</em>&nbsp;<a href="https://www.nuget.org/packages/Serilog.AspNetCore" rel="nofollow">NuGet package</a>&nbsp;into your app.</p>
<div>
<pre>dotnet add package Serilog<br />
dotnet add package Serilog.AspNetCore<br />
dotnet add package Serilog.Formatting.Elasticsearch<br />
dotnet add package Serilog.Sinks.Elasticsearch
</pre>
</div>

<p><strong>Then</strong>, add <code>UseSerilog()</code> to the Generic Host in <code>CreateHostBuilder()</code>.</p>

<div class="highlight highlight-source-cs">
  <pre>    
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
                        .Enrich.WithProperty("Application", "ModularApplication")
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                        {
                            CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                            AutoRegisterTemplate = true,
                            TemplateName = "serilog-events-template",
                            IndexFormat = "hb-cms-log-{0:yyyy.MM.dd}"
                        });
                });
    }
</pre>
</div>
