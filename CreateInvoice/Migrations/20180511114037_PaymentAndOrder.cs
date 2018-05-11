using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CreateInvoice.Migrations
{
    public partial class PaymentAndOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderNo",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIdentification",
                table: "Invoices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaymentIdentification",
                table: "Invoices");
        }
    }
}
