using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeerBook.Catalog.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BeerBook.Shared;
using BeerBook.Shared.Configuration;

namespace BeerBook.Catalog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
                .MigrateDbContext<CatalogContext>((context, services) =>
                {
                    var logger = services.GetService(typeof(ILogger<CatalogContextSeed>)) as ILogger<CatalogContextSeed>;
                    new CatalogContextSeed().Seed(context, logger).Wait();
                })
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cb => cb.AddFolder("/kv-data", "catalog"))
                .UseStartup<Startup>();
    }
}
