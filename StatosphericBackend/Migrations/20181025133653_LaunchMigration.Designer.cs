﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StatosphericBackend.Context;

namespace StatosphericBackend.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20181025133653_LaunchMigration")]
    partial class LaunchMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("StatosphericBackend.Entities.Launch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.ToTable("Launches");
                });
#pragma warning restore 612, 618
        }
    }
}
