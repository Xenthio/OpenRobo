﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenRobo.Database;

#nullable disable

namespace OpenRobo.Migrations
{
    [DbContext(typeof(ServerDatabase))]
    [Migration("20241010045028_AddUserIsMuted")]
    partial class AddUserIsMuted
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("OpenRobo.Database.User", b =>
                {
                    b.Property<ulong>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMuted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MutedUntil")
                        .HasColumnType("INTEGER");

                    b.Property<int>("XP")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
