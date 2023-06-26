﻿// <auto-generated />
using System;
using ASP_201.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ASP_201.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ASP_201.Data.Entity.EmailConfirmToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("Moment")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Used")
                        .HasColumnType("int");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.ToTable("EmailConfirmTokens");
                });

            modelBuilder.Entity("ASP_201.Data.Entity.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreateDt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeleteDt")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("ReplyId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TopicId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ReplyId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("ASP_201.Data.Entity.Rate", b =>
                {
                    b.Property<Guid>("ItemId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.HasKey("ItemId", "UserId");

                    b.ToTable("Rates");
                });

            modelBuilder.Entity("ASP_201.Data.Entity.Section", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeleteDt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UrlId")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("ASP_201.Data.Entity.Theme", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeleteDt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("SectionId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Themes");
                });

            modelBuilder.Entity("ASP_201.Data.Entity.Topic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeleteDt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("ThemeId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("ASP_201.Data.Entity.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Avatar")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EmailCode")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDatetimePublic")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsEmailPublic")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsRealNamePublic")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("LastEnterDt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RealName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("RegisterDt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ASP_201.Data.Entity.Post", b =>
                {
                    b.HasOne("ASP_201.Data.Entity.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ASP_201.Data.Entity.Post", "Reply")
                        .WithMany()
                        .HasForeignKey("ReplyId");

                    b.Navigation("Author");

                    b.Navigation("Reply");
                });

            modelBuilder.Entity("ASP_201.Data.Entity.Section", b =>
                {
                    b.HasOne("ASP_201.Data.Entity.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("ASP_201.Data.Entity.Theme", b =>
                {
                    b.HasOne("ASP_201.Data.Entity.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });
#pragma warning restore 612, 618
        }
    }
}
