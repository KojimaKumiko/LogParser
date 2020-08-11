using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;

namespace Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() {}

        public DbSet<ParsedLogFile> ParsedLogFiles { get; set; }

        public DbSet<LogFile> LogFiles { get; set; }

        public DbSet<LogPlayer> LogPlayers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"DataSource=database.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParsedLogFile>()
                .HasMany(l => l.Players)
                .WithOne(l => l.ParsedLogFile)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
