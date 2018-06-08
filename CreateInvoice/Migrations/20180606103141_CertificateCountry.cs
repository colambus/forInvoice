using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CreateInvoice.Migrations
{
    public partial class CertificateCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_Certificates_CertificateId",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_CertificateId",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "CertificateId",
                table: "Countries");

            migrationBuilder.CreateTable(
                name: "CertificateCountry",
                columns: table => new
                {
                    CertificateId = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateCountry", x => new { x.CertificateId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_CertificateCountry_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateCountry_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateCountry_CountryId",
                table: "CertificateCountry",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateCountry");

            migrationBuilder.AddColumn<int>(
                name: "CertificateId",
                table: "Countries",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CertificateId",
                table: "Countries",
                column: "CertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_Certificates_CertificateId",
                table: "Countries",
                column: "CertificateId",
                principalTable: "Certificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
