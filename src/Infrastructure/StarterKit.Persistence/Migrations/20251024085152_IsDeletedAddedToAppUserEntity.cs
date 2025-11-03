using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarterKit.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IsDeletedAddedToAppUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "AspNetUsers");
        }
    }
}
