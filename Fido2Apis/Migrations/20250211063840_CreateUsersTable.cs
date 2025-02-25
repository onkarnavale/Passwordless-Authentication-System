using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fido2Apis.Migrations
{
    /// <inheritdoc />
    public partial class CreateUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "fido2_credentials",
                newName: "user_credential_id");

            migrationBuilder.AddColumn<Guid>(
                name: "Userid",
                table: "fido2_credentials",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fido2_credentials_Userid",
                table: "fido2_credentials",
                column: "Userid");

            migrationBuilder.AddForeignKey(
                name: "FK_fido2_credentials_users_Userid",
                table: "fido2_credentials",
                column: "Userid",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fido2_credentials_users_Userid",
                table: "fido2_credentials");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "IX_fido2_credentials_Userid",
                table: "fido2_credentials");

            migrationBuilder.DropColumn(
                name: "Userid",
                table: "fido2_credentials");

            migrationBuilder.RenameColumn(
                name: "user_credential_id",
                table: "fido2_credentials",
                newName: "user_id");
        }
    }
}
