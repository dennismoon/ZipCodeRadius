using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger.Model;
using Microsoft.Extensions.PlatformAbstractions;
using NLog.Extensions.Logging;
using ZipCodeRadius.Models;
using Microsoft.EntityFrameworkCore;

namespace ZipCodeRadius
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase());

            // Configure MVC: Add XML data contract serializer, restore pascal-case naming convention
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()))
                .AddJsonOptions(o => {
                    if (o.SerializerSettings.ContractResolver != null)
                    {
                        // Remove camel-case default naming convention
                        var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
                        castedResolver.NamingStrategy = null;
                    }
                });

            // Add loggin services
            services.AddLogging();

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen();

            // Add the detail information for the API.
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Zip Code Radius Search Service",
                    Description = "Search for zip codes within a specifid number of miles.",
                    //TermsOfService = "None",
                    //Contact = new Contact { Name = "Unknown Author", Email = "", Url = "http://mycompany.com/unknown" },
                    //License = new License { Name = "Use under a license", Url = "http://url.com" }
                });

                //Determine base path for the application.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;

                //Set the comments path for the swagger json and ui.
                var xmlPath = System.IO.Path.Combine(basePath, "ZipCodeRadius.xml");
                options.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            loggerFactory.AddDebug();

            loggerFactory.AddNLog();

            // Configure nlog.config in your project root
            env.ConfigureNLog("nlog.config");

            // Configure exception handling
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            // Use built-in display of status code pages
            app.UseStatusCodePages();

            // Ensure support of static resource files
            app.UseStaticFiles();

            var context = app.ApplicationServices.GetService<ApiContext>();

            var dbInit = new InitializeDatabase(env, context);

            dbInit.Seed();

            // Add default mvc route definition
            app.UseMvcWithDefaultRoute();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi();
        }
    }
}
