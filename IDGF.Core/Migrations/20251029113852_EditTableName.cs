using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IDGF.Core.Migrations
{
    /// <inheritdoc />
    public partial class EditTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowHistories_WorkflowInstances_WorkflowInstanceId",
                table: "WorkflowHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowInstances_Reports_ReportId",
                table: "WorkflowInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowSteps_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowTransitions_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowTransitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkflowTransitions",
                table: "WorkflowTransitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkflowSteps",
                table: "WorkflowSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkflowInstances",
                table: "WorkflowInstances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkflowHistories",
                table: "WorkflowHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkflowDefinitions",
                table: "WorkflowDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.RenameTable(
                name: "WorkflowTransitions",
                newName: "opt_WorkflowTransitions");

            migrationBuilder.RenameTable(
                name: "WorkflowSteps",
                newName: "opt_WorkflowSteps");

            migrationBuilder.RenameTable(
                name: "WorkflowInstances",
                newName: "opt_WorkflowInstances");

            migrationBuilder.RenameTable(
                name: "WorkflowHistories",
                newName: "opt_WorkflowHistories");

            migrationBuilder.RenameTable(
                name: "WorkflowDefinitions",
                newName: "opt_WorkflowDefinitions");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "opt_Reports");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowTransitions_WorkflowDefinitionId",
                table: "opt_WorkflowTransitions",
                newName: "IX_opt_WorkflowTransitions_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowSteps_WorkflowDefinitionId_StepOrder",
                table: "opt_WorkflowSteps",
                newName: "IX_opt_WorkflowSteps_WorkflowDefinitionId_StepOrder");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowSteps_WorkflowDefinitionId_StepKey",
                table: "opt_WorkflowSteps",
                newName: "IX_opt_WorkflowSteps_WorkflowDefinitionId_StepKey");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowInstances_WorkflowDefinitionId",
                table: "opt_WorkflowInstances",
                newName: "IX_opt_WorkflowInstances_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowInstances_ReportId",
                table: "opt_WorkflowInstances",
                newName: "IX_opt_WorkflowInstances_ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowHistories_WorkflowInstanceId",
                table: "opt_WorkflowHistories",
                newName: "IX_opt_WorkflowHistories_WorkflowInstanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_opt_WorkflowTransitions",
                table: "opt_WorkflowTransitions",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_opt_WorkflowSteps",
                table: "opt_WorkflowSteps",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_opt_WorkflowInstances",
                table: "opt_WorkflowInstances",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_opt_WorkflowHistories",
                table: "opt_WorkflowHistories",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_opt_WorkflowDefinitions",
                table: "opt_WorkflowDefinitions",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_opt_Reports",
                table: "opt_Reports",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_opt_WorkflowHistories_opt_WorkflowInstances_WorkflowInstanceId",
                table: "opt_WorkflowHistories",
                column: "WorkflowInstanceId",
                principalTable: "opt_WorkflowInstances",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_opt_WorkflowInstances_opt_Reports_ReportId",
                table: "opt_WorkflowInstances",
                column: "ReportId",
                principalTable: "opt_Reports",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_opt_WorkflowInstances_opt_WorkflowDefinitions_WorkflowDefinitionId",
                table: "opt_WorkflowInstances",
                column: "WorkflowDefinitionId",
                principalTable: "opt_WorkflowDefinitions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_opt_WorkflowSteps_opt_WorkflowDefinitions_WorkflowDefinitionId",
                table: "opt_WorkflowSteps",
                column: "WorkflowDefinitionId",
                principalTable: "opt_WorkflowDefinitions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_opt_WorkflowTransitions_opt_WorkflowDefinitions_WorkflowDefinitionId",
                table: "opt_WorkflowTransitions",
                column: "WorkflowDefinitionId",
                principalTable: "opt_WorkflowDefinitions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_opt_WorkflowHistories_opt_WorkflowInstances_WorkflowInstanceId",
                table: "opt_WorkflowHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_opt_WorkflowInstances_opt_Reports_ReportId",
                table: "opt_WorkflowInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_opt_WorkflowInstances_opt_WorkflowDefinitions_WorkflowDefinitionId",
                table: "opt_WorkflowInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_opt_WorkflowSteps_opt_WorkflowDefinitions_WorkflowDefinitionId",
                table: "opt_WorkflowSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_opt_WorkflowTransitions_opt_WorkflowDefinitions_WorkflowDefinitionId",
                table: "opt_WorkflowTransitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_opt_WorkflowTransitions",
                table: "opt_WorkflowTransitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_opt_WorkflowSteps",
                table: "opt_WorkflowSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_opt_WorkflowInstances",
                table: "opt_WorkflowInstances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_opt_WorkflowHistories",
                table: "opt_WorkflowHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_opt_WorkflowDefinitions",
                table: "opt_WorkflowDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_opt_Reports",
                table: "opt_Reports");

            migrationBuilder.RenameTable(
                name: "opt_WorkflowTransitions",
                newName: "WorkflowTransitions");

            migrationBuilder.RenameTable(
                name: "opt_WorkflowSteps",
                newName: "WorkflowSteps");

            migrationBuilder.RenameTable(
                name: "opt_WorkflowInstances",
                newName: "WorkflowInstances");

            migrationBuilder.RenameTable(
                name: "opt_WorkflowHistories",
                newName: "WorkflowHistories");

            migrationBuilder.RenameTable(
                name: "opt_WorkflowDefinitions",
                newName: "WorkflowDefinitions");

            migrationBuilder.RenameTable(
                name: "opt_Reports",
                newName: "Reports");

            migrationBuilder.RenameIndex(
                name: "IX_opt_WorkflowTransitions_WorkflowDefinitionId",
                table: "WorkflowTransitions",
                newName: "IX_WorkflowTransitions_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_opt_WorkflowSteps_WorkflowDefinitionId_StepOrder",
                table: "WorkflowSteps",
                newName: "IX_WorkflowSteps_WorkflowDefinitionId_StepOrder");

            migrationBuilder.RenameIndex(
                name: "IX_opt_WorkflowSteps_WorkflowDefinitionId_StepKey",
                table: "WorkflowSteps",
                newName: "IX_WorkflowSteps_WorkflowDefinitionId_StepKey");

            migrationBuilder.RenameIndex(
                name: "IX_opt_WorkflowInstances_WorkflowDefinitionId",
                table: "WorkflowInstances",
                newName: "IX_WorkflowInstances_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_opt_WorkflowInstances_ReportId",
                table: "WorkflowInstances",
                newName: "IX_WorkflowInstances_ReportId");

            migrationBuilder.RenameIndex(
                name: "IX_opt_WorkflowHistories_WorkflowInstanceId",
                table: "WorkflowHistories",
                newName: "IX_WorkflowHistories_WorkflowInstanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkflowTransitions",
                table: "WorkflowTransitions",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkflowSteps",
                table: "WorkflowSteps",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkflowInstances",
                table: "WorkflowInstances",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkflowHistories",
                table: "WorkflowHistories",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkflowDefinitions",
                table: "WorkflowDefinitions",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowHistories_WorkflowInstances_WorkflowInstanceId",
                table: "WorkflowHistories",
                column: "WorkflowInstanceId",
                principalTable: "WorkflowInstances",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowInstances_Reports_ReportId",
                table: "WorkflowInstances",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowInstances",
                column: "WorkflowDefinitionId",
                principalTable: "WorkflowDefinitions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowSteps_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowSteps",
                column: "WorkflowDefinitionId",
                principalTable: "WorkflowDefinitions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowTransitions_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowTransitions",
                column: "WorkflowDefinitionId",
                principalTable: "WorkflowDefinitions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
