using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryData.Migrations
{
    public partial class ChangeISBNpropertyinBookentitytobeoftypevarchar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "LibraryAssets",
                unicode: false,
                fixedLength: true,
                maxLength: 13,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "LibraryAssets",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 13,
                oldNullable: true);
        }
    }
}
