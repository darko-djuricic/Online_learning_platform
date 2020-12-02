﻿// <auto-generated />
using System;
using WebApplication5.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WebApplication5.Migrations
{
    [DbContext(typeof(ViserCoursesContext))]
    [Migration("20200917114156_DocumentOfContent")]
    partial class DocumentOfContent
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApplication5.Models.Categories", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CategoriesId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoriesId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("WebApplication5.Models.Comment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ContentId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CourseId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsQA")
                        .HasColumnType("bit");

                    b.Property<int>("LikedComments")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("ContentId");

                    b.HasIndex("CourseId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("WebApplication5.Models.Content", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<string>("SectionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("YtVideoId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SectionId");

                    b.ToTable("Contents");
                });

            modelBuilder.Entity("WebApplication5.Models.Course", b =>
                {
                    b.Property<string>("CourseId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Keywords")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Progress")
                        .HasColumnType("int");

                    b.Property<bool>("Published")
                        .HasColumnType("bit");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime2");

                    b.HasKey("CourseId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("WebApplication5.Models.Section", b =>
                {
                    b.Property<string>("SectionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CourseId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SectionId");

                    b.HasIndex("CourseId");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("WebApplication5.Models.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("CourseId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("CourseId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApplication5.Models.Categories", b =>
                {
                    b.HasOne("WebApplication5.Models.Categories", null)
                        .WithMany("Subcategories")
                        .HasForeignKey("CategoriesId");
                });

            modelBuilder.Entity("WebApplication5.Models.Comment", b =>
                {
                    b.HasOne("WebApplication5.Models.Comment", null)
                        .WithMany("Replies")
                        .HasForeignKey("Id");

                    b.HasOne("WebApplication5.Models.Content", null)
                        .WithMany("QA")
                        .HasForeignKey("ContentId");

                    b.HasOne("WebApplication5.Models.Course", null)
                        .WithMany("Comments")
                        .HasForeignKey("CourseId");
                });

            modelBuilder.Entity("WebApplication5.Models.Content", b =>
                {
                    b.HasOne("WebApplication5.Models.Section", null)
                        .WithMany("Contents")
                        .HasForeignKey("SectionId");
                });

            modelBuilder.Entity("WebApplication5.Models.Course", b =>
                {
                    b.HasOne("WebApplication5.Models.Categories", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebApplication5.Models.Section", b =>
                {
                    b.HasOne("WebApplication5.Models.Course", null)
                        .WithMany("Sections")
                        .HasForeignKey("CourseId");
                });

            modelBuilder.Entity("WebApplication5.Models.User", b =>
                {
                    b.HasOne("WebApplication5.Models.Course", null)
                        .WithMany("Participants")
                        .HasForeignKey("CourseId");
                });
#pragma warning restore 612, 618
        }
    }
}
