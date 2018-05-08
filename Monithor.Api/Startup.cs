using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monithor.Api.Hub;
using Monithor.Components;
using Monithor.Ports;

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

            services.AddSingleton<IMessageHandler>(provider => 
                new MessageHandler(
                    provider.GetService<IHub>(),
                    provider.GetService<ITraceStorage>(),
                    TimeSpan.FromMilliseconds(1000),
                    TimeSpan.FromMilliseconds(10000)));


            services.AddMvc();
            services.AddSignalR();

            var builtProvider = services.BuildServiceProvider();
            builtProvider.GetService<MessageHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
        }
    }
}
