using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PmsApi.Migrations
{
    /// <inheritdoc />
    public partial class EmailKeyUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "users_ibfk_1",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_role_id",
                table: "users",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_role_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "email",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "users_ibfk_1",
                table: "users",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id");
        }
    }
}
