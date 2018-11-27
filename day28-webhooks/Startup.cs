using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<HttpClient>(GraphHttpClientFactory.GetAuthenticatedHTTPClient(Configuration));
            services.AddSingleton<ISubscriptionRepository>(new SubscriptionRepository());
            services.AddSingleton<NotificationUrl>(new NotificationUrl { Url = $"{Configuration["baseUrl"]}/api/notifications" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseMvc();
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
