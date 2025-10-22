using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncClaimDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaimDocuments_Claims_ClaimId",
                table: "ClaimDocuments");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropIndex(
                name: "IX_ClaimDocuments_ClaimId",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "ClaimId",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ClaimDocuments");

            migrationBuilder.RenameColumn(
                name: "StoredPath",
                table: "ClaimDocuments",
                newName: "Status");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ClaimDocuments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EvidenceFileName",
                table: "ClaimDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "ClaimDocuments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HoursWorked",
                table: "ClaimDocuments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MentorName",
                table: "ClaimDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ClaimDocuments",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAtUtc",
                table: "ClaimDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "ClaimDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "ClaimDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewNote",
                table: "ClaimDocuments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "EvidenceFileName",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "HoursWorked",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "MentorName",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "PaidAtUtc",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "ClaimDocuments");

            migrationBuilder.DropColumn(
                name: "ReviewNote",
                table: "ClaimDocuments");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ClaimDocuments",
                newName: "StoredPath");

            migrationBuilder.AddColumn<int>(
                name: "ClaimId",
                table: "ClaimDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ClaimDocuments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ClaimDocuments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HoursWorked = table.Column<double>(type: "float", nullable: false),
                    LecturerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_ClaimId",
                table: "ClaimDocuments",
                column: "ClaimId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClaimDocuments_Claims_ClaimId",
                table: "ClaimDocuments",
                column: "ClaimId",
                principalTable: "Claims",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
