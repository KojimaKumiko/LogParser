﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("Database.Models.LogFile", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BossName")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ParsedLogFileID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Recorder")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("ParsedLogFileID");

                    b.ToTable("LogFile");
                });

            modelBuilder.Entity("Database.Models.LogPlayer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountName")
                        .HasColumnType("TEXT");

                    b.Property<int>("Concentration")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Condition")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasCommander")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Healing")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Instance")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("ParsedLogFileID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SubGroup")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Toughness")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Weapons")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("ParsedLogFileID");

                    b.ToTable("LogPlayer");
                });

            modelBuilder.Entity("Database.Models.ParsedLogFile", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ArcVersion")
                        .HasColumnType("TEXT");

                    b.Property<string>("BossIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("BossName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Duration")
                        .HasColumnType("TEXT");

                    b.Property<string>("EliteInsightsVersion")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<long>("Gw2Build")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCM")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<long>("LanguageID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RecordedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Success")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TriggerID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("ParsedLogFile");
                });

            modelBuilder.Entity("Database.Models.LogFile", b =>
                {
                    b.HasOne("Database.Models.ParsedLogFile", "ParsedLogFile")
                        .WithMany()
                        .HasForeignKey("ParsedLogFileID");
                });

            modelBuilder.Entity("Database.Models.LogPlayer", b =>
                {
                    b.HasOne("Database.Models.ParsedLogFile", "ParsedLogFile")
                        .WithMany("Players")
                        .HasForeignKey("ParsedLogFileID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
