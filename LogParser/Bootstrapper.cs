using Database;
using LogParser.ViewModels;
using Stylet;
using StyletIoC;

namespace LogParser
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
#if DEBUG
            using DatabaseContext db = new DatabaseContext();
            DatabaseSeeder.Seed(db);
#endif
        }
    }
}
