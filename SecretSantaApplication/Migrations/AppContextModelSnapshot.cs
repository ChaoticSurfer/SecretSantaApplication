﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SecretSantaApplication.Data;

namespace SecretSantaApplication.Migrations
{
    [DbContext(typeof(AppContext))]
    partial class AppContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("SecretSantaApplication.Models.Profile", b =>
                {
                    b.Property<string>("EmailAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("BirthDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("LetterToSecretSanta")
                        .HasColumnType("TEXT");

                    b.HasKey("EmailAddress");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("SecretSantaApplication.Models.User", b =>
                {
                    b.Property<string>("EmailAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.HasKey("EmailAddress");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
