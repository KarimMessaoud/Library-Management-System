using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryData.Migrations
{
    public partial class ConfigureHoldentitywithFluentAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Holds_LibraryAssets_LibraryAssetId",
                table: "Holds");

            migrationBuilder.DropForeignKey(
                name: "FK_Holds_LibraryCards_LibraryCardId",
                table: "Holds");

            migrationBuilder.DropIndex(
                name: "IX_Holds_LibraryAssetId",
                table: "Holds");

            migrationBuilder.AlterColumn<int>(
                name: "LibraryCardId",
                table: "Holds",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LibraryAssetId",
                table: "Holds",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Holds_LibraryAssetId",
                table: "Holds",
                column: "LibraryAssetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Holds_LibraryAssets_LibraryAssetId",
                table: "Holds",
                column: "LibraryAssetId",
                principalTable: "LibraryAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Holds_LibraryCards_LibraryCardId",
                table: "Holds",
                column: "LibraryCardId",
                principalTable: "LibraryCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Holds_LibraryAssets_LibraryAssetId",
                table: "Holds");

            migrationBuilder.DropForeignKey(
                name: "FK_Holds_LibraryCards_LibraryCardId",
                table: "Holds");

            migrationBuilder.DropIndex(
                name: "IX_Holds_LibraryAssetId",
                table: "Holds");

            migrationBuilder.AlterColumn<int>(
                name: "LibraryCardId",
                table: "Holds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LibraryAssetId",
                table: "Holds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Holds_LibraryAssetId",
                table: "Holds",
                column: "LibraryAssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Holds_LibraryAssets_LibraryAssetId",
                table: "Holds",
                column: "LibraryAssetId",
                principalTable: "LibraryAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Holds_LibraryCards_LibraryCardId",
                table: "Holds",
                column: "LibraryCardId",
                principalTable: "LibraryCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
