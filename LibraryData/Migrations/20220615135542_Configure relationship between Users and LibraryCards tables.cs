using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryData.Migrations
{
    public partial class ConfigurerelationshipbetweenUsersandLibraryCardstables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryCards_AspNetUsers_PatronId",
                table: "LibraryCards");

            migrationBuilder.DropIndex(
                name: "IX_LibraryCards_PatronId",
                table: "LibraryCards");

            migrationBuilder.DropColumn(
                name: "PatronId",
                table: "LibraryCards");

            migrationBuilder.AddColumn<int>(
                name: "LibraryCardId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LibraryCardId",
                table: "AspNetUsers",
                column: "LibraryCardId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_LibraryCards_LibraryCardId",
                table: "AspNetUsers",
                column: "LibraryCardId",
                principalTable: "LibraryCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_LibraryCards_LibraryCardId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LibraryCardId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LibraryCardId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "PatronId",
                table: "LibraryCards",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LibraryCards_PatronId",
                table: "LibraryCards",
                column: "PatronId",
                unique: true,
                filter: "[PatronId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryCards_AspNetUsers_PatronId",
                table: "LibraryCards",
                column: "PatronId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
