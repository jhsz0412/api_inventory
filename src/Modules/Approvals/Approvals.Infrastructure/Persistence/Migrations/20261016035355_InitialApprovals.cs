using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Approvals.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialApprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequisitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActorEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    SequenceOrder = table.Column<int>(type: "int", nullable: true),
                    PreviousStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NewStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentApprovalWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentApprovalWorkflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequisitionApprovals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionApprovals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentApprovalWorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentApprovalWorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproverEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentApprovalWorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentApprovalWorkflowSteps_DepartmentApprovalWorkflows_DepartmentApprovalWorkflowId",
                        column: x => x.DepartmentApprovalWorkflowId,
                        principalTable: "DepartmentApprovalWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequisitionApprovalSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequisitionApprovalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproverEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceOrder = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ActedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionApprovalSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitionApprovalSteps_RequisitionApprovals_RequisitionApprovalId",
                        column: x => x.RequisitionApprovalId,
                        principalTable: "RequisitionApprovals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalAuditLogs_ActorEmployeeId_OccurredAtUtc",
                table: "ApprovalAuditLogs",
                columns: new[] { "ActorEmployeeId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalAuditLogs_DepartmentId_OccurredAtUtc",
                table: "ApprovalAuditLogs",
                columns: new[] { "DepartmentId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalAuditLogs_RequisitionId_OccurredAtUtc",
                table: "ApprovalAuditLogs",
                columns: new[] { "RequisitionId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentApprovalWorkflows_DepartmentId_IsActive",
                table: "DepartmentApprovalWorkflows",
                columns: new[] { "DepartmentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentApprovalWorkflowSteps_DepartmentApprovalWorkflowId_ApproverEmployeeId",
                table: "DepartmentApprovalWorkflowSteps",
                columns: new[] { "DepartmentApprovalWorkflowId", "ApproverEmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentApprovalWorkflowSteps_DepartmentApprovalWorkflowId_SequenceOrder",
                table: "DepartmentApprovalWorkflowSteps",
                columns: new[] { "DepartmentApprovalWorkflowId", "SequenceOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionApprovals_RequisitionId",
                table: "RequisitionApprovals",
                column: "RequisitionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionApprovalSteps_RequisitionApprovalId_SequenceOrder",
                table: "RequisitionApprovalSteps",
                columns: new[] { "RequisitionApprovalId", "SequenceOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalAuditLogs");

            migrationBuilder.DropTable(
                name: "DepartmentApprovalWorkflowSteps");

            migrationBuilder.DropTable(
                name: "RequisitionApprovalSteps");

            migrationBuilder.DropTable(
                name: "DepartmentApprovalWorkflows");

            migrationBuilder.DropTable(
                name: "RequisitionApprovals");
        }
    }
}
