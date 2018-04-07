using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Autofac;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace JobModel.AutoFac
{
    public class AutoFacContainer
    {
        public AutoFacContainer()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile($"{AppContext.BaseDirectory}/appsettings.json");
            Configuration = configBuilder.Build();

            ContainerBuilder = new ContainerBuilder();

            var serilogOptions = Configuration.GetSection(nameof(SerilogOptions)).Get<SerilogOptions>();

            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.MinimumLevel.Debug();
            loggerConfiguration.WriteTo.RollingFile("logs/{Date}.txt", serilogOptions.RollingFileLogEventLevel);
            loggerConfiguration.WriteTo.ColoredConsole(serilogOptions.ConsoleLogEventLevel);

            var logger = loggerConfiguration.CreateLogger();

            ContainerBuilder.RegisterInstance<ILogger>(logger);
            
            //load arango setting
            var arangoOptions = Configuration.GetSection(nameof(ArangoOptions)).Get<ArangoOptions>();
            ContainerBuilder.RegisterInstance<ArangoConnection>(new ArangoConnection(arangoOptions));
        }

        public IConfiguration Configuration { get; set; }

        public ContainerBuilder ContainerBuilder { get; set; }
    }
}
