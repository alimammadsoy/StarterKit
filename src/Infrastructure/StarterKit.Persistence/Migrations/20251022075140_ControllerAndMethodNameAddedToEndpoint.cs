using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarterKit.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ControllerAndMethodNameAddedToEndpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "action_name",
                table: "endpoints",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "controller_name",
                table: "endpoints",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "action_name",
                table: "endpoints");

            migrationBuilder.DropColumn(
                name: "controller_name",
                table: "endpoints");
        }
    }
}
