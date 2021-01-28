using Database;
using LogParser.Services;
using LogParser.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stylet;
using StyletIoC;
using System;
using System.IO;
using Serilog;
using System.Threading.Tasks;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;

namespace LogParser
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        private IConfiguration configuration;

        private DatabaseContext database;

        private SnackbarMessageQueue messageQueue;

        public override void Dispose()
        {
            messageQueue?.Dispose();
            database?.Dispose();

            base.Dispose();
            GC.SuppressFinalize(this);
        }

        protected override void OnStart()
        {
            string environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

            configuration = config.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            optionsBuilder.UseSqlite(configuration.GetConnectionString("Database"));

            database = new DatabaseContext(optionsBuilder.Options);

            SetupExceptionHandling();

            messageQueue = new SnackbarMessageQueue();

            Log.Debug("Configuration done");
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            Log.Debug("Configuring IoC");

            // Instance bindings
            builder.Bind<IConfiguration>().ToInstance(configuration);
            builder.Bind<DatabaseContext>().ToInstance(database).DisposeWithContainer(false);
            builder.Bind<SnackbarMessageQueue>().ToInstance(messageQueue).DisposeWithContainer(false);

            // Service/Controller bindings
            builder.Bind<DpsReportService>().ToSelf();
            builder.Bind<IParseService>().To<ParseService>();
            builder.Bind<IDiscordService>().To<DiscordService>();

            // ViewModel bindings
            builder.Bind<LogParserViewModel>().ToSelf();
            builder.Bind<SettingsViewModel>().ToSelf();
            builder.Bind<AboutViewModel>().ToSelf();
            builder.Bind<LogFilesViewModel>().ToSelf();
        }

        protected override void Configure()
        {
            Log.Debug("Seeding database");
            DatabaseSeeder.Seed(database);
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Application.Current.DispatcherUnhandledException");
            e.Handled = true;
        }

        private static void SetupExceptionHandling()
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
