using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fido2Apis.Migrations
{
    /// <inheritdoc />
    public partial class CreateFido2CredentialsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fido2_credentials",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<byte[]>(type: "bytea", nullable: false),
                    public_key = table.Column<byte[]>(type: "bytea", nullable: false),
                    user_handle = table.Column<byte[]>(type: "bytea", nullable: false),
                    signature_counter = table.Column<long>(type: "bigint", nullable: false),
                    cred_type = table.Column<string>(type: "text", nullable: false),
                    reg_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    aaguid = table.Column<Guid>(type: "uuid", nullable: false),
                    descriptor_json = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fido2_credentials", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fido2_credentials");
        }
    }
}
