using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonnelAccessManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeNo",
                table: "Events");

            migrationBuilder.Sql(
                """
                ALTER TABLE "Events" 
                ALTER COLUMN "EventType" TYPE integer 
                USING "EventType"::integer;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "CorrelationId",
                table: "Events",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "FailCount",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SourceDetail",
                table: "Events",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceId",
                table: "Events",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SuccessCount",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalCount",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                ALTER TABLE "EventLogs" 
                ALTER COLUMN "Status" TYPE integer 
                USING "Status"::integer;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "EventLogs",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "EventLogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "EmployeeNo",
                table: "EventLogs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PersonnelName",
                table: "EventLogs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "RoleId",
                table: "EventLogs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RoleName",
                table: "EventLogs",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CorrelationId",
                table: "Events",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventType_IsCompleted",
                table: "Events",
                columns: new[] { "EventType", "IsCompleted" });

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_EmployeeNo",
                table: "EventLogs",
                column: "EmployeeNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_CorrelationId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_EventType_IsCompleted",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_EventLogs_EmployeeNo",
                table: "EventLogs");

            migrationBuilder.DropColumn(
                name: "FailCount",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SourceDetail",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SuccessCount",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TotalCount",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Action",
                table: "EventLogs");

            migrationBuilder.DropColumn(
                name: "EmployeeNo",
                table: "EventLogs");

            migrationBuilder.DropColumn(
                name: "PersonnelName",
                table: "EventLogs");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "EventLogs");

            migrationBuilder.DropColumn(
                name: "RoleName",
                table: "EventLogs");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "Events",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "CorrelationId",
                table: "Events",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeNo",
                table: "Events",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "EventLogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "EventLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
