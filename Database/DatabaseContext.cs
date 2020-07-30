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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"DataSource=database.db;");
        }
    }
}
