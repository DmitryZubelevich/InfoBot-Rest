using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using InfoBot.MessageGenerators.WeatherMessageGenerator;
using Telegram.Bot;
using Loggly.Config;
using Loggly;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using InfoBot.Infrastructure.Filters;

namespace InfoBot
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options => options.Filters.Add(typeof(HttpGlobalExceptionFilter)));
            services.AddSingleton<IWeatherMessageGenerator>(new WeatherMessageGenerator(Configuration["ApiKeys:DarkSky"]));
            var botClient = new TelegramBotClient(Configuration["ApiKeys:Telegram"]);
            botClient.SetWebhookAsync($"{Configuration["AppSettings:AppUrl"]}/api/webhook").Wait();
            services.AddSingleton<ITelegramBotClient>(botClient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            SetupLogglyConfiguration();
            Log.Logger = CreateLogger();
            loggerFactory.AddSerilog();

            app.UseMvc();
        }

        private Logger CreateLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Loggly(LogEventLevel.Warning)
                .CreateLogger();
        }

        private void SetupLogglyConfiguration()
        {
            //CHANGE THESE TWO TO YOUR LOGGLY ACCOUNT: DO NOT COMMIT TO Source control!!!
            var appName = Configuration["Logging:Loggly:AppName"];
            var customerToken = Configuration["Logging:Loggly:CustomerToken"];

            //Configure Loggly
            var config = LogglyConfig.Instance;
            config.CustomerToken = customerToken;
            config.ApplicationName = appName;
            config.Transport = new TransportConfiguration()
            {
                EndpointHostname = Configuration["Logging:Loggly:Host"],
                EndpointPort = 443,
                LogTransport = LogTransport.Https
            };
            config.ThrowExceptions = true;

            //Define Tags sent to Loggly
            config.TagConfig.Tags.AddRange(new ITag[]{
                new ApplicationNameTag {Formatter = "application-{0}"},
                new HostnameTag { Formatter = "host-{0}" }
            });
        }
    }
}
