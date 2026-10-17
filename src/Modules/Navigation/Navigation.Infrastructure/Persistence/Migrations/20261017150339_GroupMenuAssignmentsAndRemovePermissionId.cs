using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Navigation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GroupMenuAssignmentsAndRemovePermissionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "MenuItems");

            migrationBuilder.CreateTable(
                name: "GroupMenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMenuItems_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupMenuItems_GroupId_MenuItemId",
                table: "GroupMenuItems",
                columns: new[] { "GroupId", "MenuItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupMenuItems_MenuItemId",
                table: "GroupMenuItems",
                column: "MenuItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupMenuItems");

            migrationBuilder.AddColumn<Guid>(
                name: "PermissionId",
                table: "MenuItems",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
