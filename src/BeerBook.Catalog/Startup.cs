using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeerBook.Catalog.Extensions;
using BeerBook.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace BeerBook.Catalog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDatabase(Configuration)
                .AddSwaggerGen(options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = "Catalog HTTP API",
                        Version = "v1",
                        Description = "The Catalog Service HTTP API",
                        TermsOfService = "Terms Of Service"
                    });
                })
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseConfiguratedPathBase(Configuration, loggerFactory);

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    var path = Configuration.GetBasePath();
                    c.SwaggerEndpoint($"{path}/swagger/v1/swagger.json", "Catalog.API V1");
                });

            app.UseMvc();
        }
    }
}
