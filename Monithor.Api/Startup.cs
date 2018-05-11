using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monithor.Api.Hub;
using Monithor.Api.Logging;
using Monithor.Components;
using Monithor.Ports;
using SignalRHelper.Server;

namespace Monithor.Api
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader()
                        .WithOrigins("http://localhost:52903")
                        .AllowCredentials();
                }));

            services.AddSingleton<ITraceStorage, MemoryStorage>();
            services.AddSingleton<IHub, HubInterface>();


            var logger = new SimpleLogger();


            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<ILogCollector>(logger);

            services.AddSingleton<DisconnectionDetector>();

            services.AddSingleton<IMessageHandler>(provider =>
                new MessageHandler(
                    provider.GetService<IHub>(),
                    provider.GetService<ITraceStorage>(),
                    provider.GetService<ILogger>()));


            services.AddMvc();
            services.AddSignalR();

            logger.Log("Application started");

            HubConnectionManager.Instance.ClientConnected += (c) => { logger.Log($"client connected {c.Id}"); };
            HubConnectionManager.Instance.ClientDisconnected += (c) => { logger.Log($"client disconnected {c.Id}"); };
            HubConnectionManager.Instance.ClientConnectionStatusChanged += (c) => { logger.Log($"client connection status changed {c.Id}. {c.ConnectionStatus}"); };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSignalR(route =>
            {
                route.MapHub<ThorHub>("/thorhub");
            });
            app.UseCors("CorsPolicy");
            app.UseMvc();

            serviceProvider.GetService<DisconnectionDetector>().Start();
        }
    }
}
