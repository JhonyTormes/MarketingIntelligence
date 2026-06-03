using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignAndUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CampaignName",
                schema: "link_shortener",
                table: "ShortenedLinks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "link_shortener",
                table: "ShortenedLinks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampaignName",
                schema: "link_shortener",
                table: "ShortenedLinks");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "link_shortener",
                table: "ShortenedLinks");
        }
    }
}
