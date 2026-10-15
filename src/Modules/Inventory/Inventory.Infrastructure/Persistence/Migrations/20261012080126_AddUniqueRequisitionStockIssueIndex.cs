using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueRequisitionStockIssueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InventoryStockTransactions_ReferenceNumber_InventoryItemId_TransactionType",
                table: "InventoryStockTransactions",
                columns: new[] { "ReferenceNumber", "InventoryItemId", "TransactionType" },
                unique: true,
                filter: "[TransactionType] = 2 AND [ReferenceNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryStockTransactions_ReferenceNumber_InventoryItemId_TransactionType",
                table: "InventoryStockTransactions");
        }
    }
}
