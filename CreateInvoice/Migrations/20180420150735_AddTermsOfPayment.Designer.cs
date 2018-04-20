﻿// <auto-generated />
using CreateInvoice.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace CreateInvoice.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20180420150735_AddTermsOfPayment")]
    partial class AddTermsOfPayment
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CreateInvoice.Entities.Contract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BuyerId");

                    b.Property<string>("Name");

                    b.Property<int?>("SellerId");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.HasIndex("SellerId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("CreateInvoice.Entities.DeliveryType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("DeliveryTypes");
                });

            modelBuilder.Entity("CreateInvoice.Entities.Organization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("CreateInvoice.Entities.TermOfPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("TermsOfPayment");
                });

            modelBuilder.Entity("CreateInvoice.Entities.TermsOfDelivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("TermsOfDelivery");
                });

            modelBuilder.Entity("CreateInvoice.Entities.Contract", b =>
                {
                    b.HasOne("CreateInvoice.Entities.Organization", "Buyer")
                        .WithMany()
                        .HasForeignKey("BuyerId");

                    b.HasOne("CreateInvoice.Entities.Organization", "Seller")
                        .WithMany()
                        .HasForeignKey("SellerId");
                });
#pragma warning restore 612, 618
        }
    }
}
