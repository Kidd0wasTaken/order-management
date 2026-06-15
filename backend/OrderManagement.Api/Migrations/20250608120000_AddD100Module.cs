using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OrderManagement.Api.Migrations;

public partial class AddD100Module : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CompanyProfiles",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Cui = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                Denumire = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Adresa = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                Telefon = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                NumeDeclar = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                PrenumeDeclar = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                FunctieDeclar = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CompanyProfiles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "D100Declarations",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Luna = table.Column<int>(type: "integer", nullable: false),
                An = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                OrderCount = table.Column<int>(type: "integer", nullable: false),
                TotalSales = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                SumaDat = table.Column<long>(type: "bigint", nullable: false),
                SumaDed = table.Column<long>(type: "bigint", nullable: false),
                SumaPlata = table.Column<long>(type: "bigint", nullable: false),
                SumaRest = table.Column<long>(type: "bigint", nullable: false),
                TotalPlataA = table.Column<long>(type: "bigint", nullable: false),
                CodOblig = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                CodBugetar = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                Scadenta = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                NrEvidenta = table.Column<string>(type: "character varying(23)", maxLength: 23, nullable: false),
                XmlPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                PdfPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                ValidationErrors = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_D100Declarations", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_D100Declarations_Luna_An",
            table: "D100Declarations",
            columns: new[] { "Luna", "An" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Orders_Status",
            table: "Orders",
            column: "Status");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "D100Declarations");
        migrationBuilder.DropTable(name: "CompanyProfiles");
        migrationBuilder.DropIndex(name: "IX_Orders_Status", table: "Orders");
    }
}
