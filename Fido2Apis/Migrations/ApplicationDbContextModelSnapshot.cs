﻿// <auto-generated />
using System;
using Fido2Apis.Infra.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fido2Apis.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Fido2Apis.Domain.Fido2Auth.Fido2Credential", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("AaGuid")
                        .HasColumnType("uuid")
                        .HasColumnName("aaguid");

                    b.Property<string>("CredType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("cred_type");

                    b.Property<string>("DescriptorJson")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("descriptor_json");

                    b.Property<byte[]>("PublicKey")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("public_key");

                    b.Property<DateTime>("RegDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("reg_date");

                    b.Property<long>("SignatureCounter")
                        .HasColumnType("bigint")
                        .HasColumnName("signature_counter");

                    b.Property<byte[]>("UserHandle")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("user_handle");

                    b.Property<byte[]>("UserId")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("user_credential_id");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.Property<Guid?>("Userid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Userid");

                    b.ToTable("fido2_credentials");
                });

            modelBuilder.Entity("Fido2Apis.Domain.User.User", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("Fido2Apis.Domain.Fido2Auth.Fido2Credential", b =>
                {
                    b.HasOne("Fido2Apis.Domain.User.User", null)
                        .WithMany("Fido2Credentials")
                        .HasForeignKey("Userid");
                });

            modelBuilder.Entity("Fido2Apis.Domain.User.User", b =>
                {
                    b.Navigation("Fido2Credentials");
                });
#pragma warning restore 612, 618
        }
    }
}
