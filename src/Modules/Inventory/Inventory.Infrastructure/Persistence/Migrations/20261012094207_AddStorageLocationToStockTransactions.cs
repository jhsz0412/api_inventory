using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageLocationToStockTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StorageLocationId",
                table: "InventoryStockTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStockTransactions_StorageLocationId",
                table: "InventoryStockTransactions",
                column: "StorageLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStockTransactions_StorageLocations_StorageLocationId",
                table: "InventoryStockTransactions",
                column: "StorageLocationId",
                principalTable: "StorageLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStockTransactions_StorageLocations_StorageLocationId",
                table: "InventoryStockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryStockTransactions_StorageLocationId",
                table: "InventoryStockTransactions");

            migrationBuilder.DropColumn(
                name: "StorageLocationId",
                table: "InventoryStockTransactions");
        }
    }
}
