using Fanex.Logging.Distributor;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.UseUrls("http://0.0.0.0:5000");
    })
    .Build()
    .RunAsync();
