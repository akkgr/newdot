using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using ChatApplication;
using Cinnamon.Auth;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using newdot.Models;
using Serilog;
using WebSocketManager;

namespace newdot
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        private string dbConectionString { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            dbConectionString = Configuration.GetConnectionString("DefaultConnection");
            Models.Context.Init();
            var ctx = new Context(dbConectionString);
            ctx.EnsureIndexes();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Context>(p => new Context(dbConectionString));

            services.AddIdentityServer()
                .AddSigningCredential(new X509Certificate2(Path.Combine(".", "certs", "IdentityServer4Auth.pfx"), "", X509KeyStorageFlags.MachineKeySet))
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddProfileService<ProfileService>()
                .AddResourceOwnerValidator<PasswordValidator>();

            services.AddHangfire(x => x.UseMongoStorage(dbConectionString, "logs"));
            services.AddWebSocketManager();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddSerilog();

            app.UseIdentityServer();

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:5000",
                RequireHttpsMetadata = false,

                ApiName = "api1"
            });

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseWebSockets();

            app.MapWebSocketManager("/chat", serviceProvider.GetService<ChatApplication.ChatHandler>());

            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}
