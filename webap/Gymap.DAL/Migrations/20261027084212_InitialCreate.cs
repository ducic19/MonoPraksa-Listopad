using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymap.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "subscription",
                columns: table => new
                {
                    subs_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    weekly = table.Column<int>(type: "integer", nullable: false),
                    duration = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subscription_pkey", x => x.subs_id);
                });

            migrationBuilder.CreateTable(
                name: "trainer",
                columns: table => new
                {
                    trainer_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("trainer_pkey", x => x.trainer_id);
                });

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    mem_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    subs_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("member_pkey", x => x.mem_id);
                    table.ForeignKey(
                        name: "fk_subs_id",
                        column: x => x.subs_id,
                        principalTable: "subscription",
                        principalColumn: "subs_id");
                });

            migrationBuilder.CreateTable(
                name: "trainer_subs",
                columns: table => new
                {
                    subs_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trainer_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("trainer_subs_pkey", x => new { x.subs_id, x.trainer_id });
                    table.ForeignKey(
                        name: "fk_ts_s",
                        column: x => x.subs_id,
                        principalTable: "subscription",
                        principalColumn: "subs_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ts_t",
                        column: x => x.trainer_id,
                        principalTable: "trainer",
                        principalColumn: "trainer_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_member_subs_id",
                table: "member",
                column: "subs_id");

            migrationBuilder.CreateIndex(
                name: "ux_member_email",
                table: "member",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trainer_subs_trainer_id",
                table: "trainer_subs",
                column: "trainer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "member");

            migrationBuilder.DropTable(
                name: "trainer_subs");

            migrationBuilder.DropTable(
                name: "subscription");

            migrationBuilder.DropTable(
                name: "trainer");
        }
    }
}
