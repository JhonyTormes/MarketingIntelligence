using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialLinkShortenerMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "link_shortener");

            migrationBuilder.CreateSequence(
                name: "ShortenedLinkSequence",
                schema: "link_shortener");

            migrationBuilder.CreateTable(
                name: "LinkClicks",
                schema: "link_shortener",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShortenedLinkId = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    ClickedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkClicks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShortenedLinks",
                schema: "link_shortener",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalUrl = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "text", nullable: false),
                    SequenceId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('link_shortener.\"ShortenedLinkSequence\"')"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortenedLinks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkClicks_ClickedAt",
                schema: "link_shortener",
                table: "LinkClicks",
                column: "ClickedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LinkClicks_ShortenedLinkId",
                schema: "link_shortener",
                table: "LinkClicks",
                column: "ShortenedLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedLinks_OriginalUrl",
                schema: "link_shortener",
                table: "ShortenedLinks",
                column: "OriginalUrl");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedLinks_ShortCode",
                schema: "link_shortener",
                table: "ShortenedLinks",
                column: "ShortCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkClicks",
                schema: "link_shortener");

            migrationBuilder.DropTable(
                name: "ShortenedLinks",
                schema: "link_shortener");

            migrationBuilder.DropSequence(
                name: "ShortenedLinkSequence",
                schema: "link_shortener");
        }
    }
}
