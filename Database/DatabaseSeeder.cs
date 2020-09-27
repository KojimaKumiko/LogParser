using Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Database
{
    public static class DatabaseSeeder
    {
        public static void Seed(DatabaseContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

#if DEBUG
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
#else
            context.Database.Migrate();
#endif
            //string json = File.ReadAllText(@"SeedData\20200717-195820_vg_kill.json");
            //context.Add(new ParsedLogFile { BossName = "Vale Guardian", Recorder = "Liz Monsuta", Json = json});
            //context.SaveChanges();
        }
    }
}
