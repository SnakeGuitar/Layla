using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Layla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectModel_20260315174059 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsPublic",
                table: "Projects",
                column: "IsPublic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_IsPublic",
                table: "Projects");
        }
    }
}
