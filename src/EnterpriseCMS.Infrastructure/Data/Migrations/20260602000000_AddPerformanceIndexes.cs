using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseCMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Contents_TenantId_Status_PublishedAt",
                table: "Contents",
                columns: new[] { "TenantId", "Status", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_TenantId_FolderId",
                table: "MediaAssets",
                columns: new[] { "TenantId", "FolderId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "TenantId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contents_TenantId_Status_PublishedAt",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_MediaAssets_TenantId_FolderId",
                table: "MediaAssets");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_TenantId_CreatedAt",
                table: "AuditLogs");
        }
    }
}
