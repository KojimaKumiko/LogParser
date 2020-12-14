using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<ParsedLogFile> ParsedLogFiles { get; set; }

        public DbSet<LogPlayer> LogPlayers { get; set; }

        public DbSet<DpsTarget> DpsTargets { get; set; }

        public DbSet<Setting> Settings { get; set; }

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "The method is provied by Entity Framework Core and gets called by it.")]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParsedLogFile>()
                .HasMany(l => l.Players)
                .WithOne(l => l.ParsedLogFile)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogPlayer>()
                .HasMany(p => p.DpsTargets)
                .WithOne(d => d.Player)
                .HasForeignKey(d => d.PlayerID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
