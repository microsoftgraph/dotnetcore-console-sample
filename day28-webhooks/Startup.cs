using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GraphWebhooks
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();
            services.AddSingleton<HttpClient>(GraphHttpClientFactory.GetAuthenticatedHTTPClient(Configuration));
            services.AddSingleton<ISubscriptionRepository>(new SubscriptionRepository());
            services.AddSingleton<NotificationUrl>(new NotificationUrl { Url = $"{Configuration["baseUrl"]}/api/notifications" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ValidateConfig();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ValidateConfig()
        {
            // Validate required settings
            if (string.IsNullOrEmpty(Configuration["applicationId"]) ||
                string.IsNullOrEmpty(Configuration["applicationSecret"]) ||
                string.IsNullOrEmpty(Configuration["redirectUri"]) ||
                string.IsNullOrEmpty(Configuration["tenantId"]) ||
                string.IsNullOrEmpty(Configuration["baseUrl"]))
            {
                throw new ApplicationException("The configuration is invalid, are you missing some keys?");
            }
        }
    }
}
