using Database;
using LogParser.Controller;
using LogParser.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stylet;
using StyletIoC;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Serilog;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LogParser
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        private IConfiguration configuration;

        private DatabaseContext database;

        private bool isDev;

        protected override void OnStart()
        {
            //string environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            string environmentName = "Production";

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

            configuration = config.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            if (environmentName.Equals("Dev", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Database"));
                isDev = true;
            }
            else
            {
                optionsBuilder.UseSqlite(configuration.GetConnectionString("Database"));
                isDev = false;
            }

            database = new DatabaseContext(optionsBuilder.Options);

            SetupExceptionHandling();

            Log.Debug("Configuration done.");
        }

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "The method and it's parameter is provided by Stylet and get's called from it.")]
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            Log.Debug("Configuring IoC.");

            // Configure the IoC container in here
            builder.Bind<IConfiguration>().ToInstance(configuration);
            builder.Bind<DatabaseContext>().ToInstance(database);
            builder.Bind<ParseController>().ToSelf();
            builder.Bind<DpsReportController>().ToSelf();

            // Factories for ViewModels
            builder.Bind<LogParserViewModel>().ToFactory(c => new LogParserViewModel(c.Get<DatabaseContext>(), c.Get<ParseController>(), c.Get<DpsReportController>()));
            builder.Bind<SettingsViewModel>().ToFactory(c => new SettingsViewModel(c.Get<DatabaseContext>(), c.Get<DpsReportController>()));
        }

        protected override void Configure()
        {
            Log.Debug("Seeding database.");
            DatabaseSeeder.Seed(database, isDev);
        }

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "The hook get's called through Stylets bootstrapper.")]
        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Application.Current.DispatcherUnhandledException");
            e.Handled = true;
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                Log.Error((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Log.Error(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }
    }
}
