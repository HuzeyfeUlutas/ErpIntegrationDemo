using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonnelAccessManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesAndColumnsToPascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_personnel_scheduled_actions",
                table: "personnel_scheduled_actions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_kafka_event_logs",
                table: "kafka_event_logs");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "personnel_scheduled_actions",
                newName: "PersonnelScheduledActions");

            migrationBuilder.RenameTable(
                name: "kafka_event_logs",
                newName: "KafkaEventLogs");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_token",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_token");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_employee_no",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_employee_no");

            migrationBuilder.RenameIndex(
                name: "IX_personnel_scheduled_actions_event_id",
                table: "PersonnelScheduledActions",
                newName: "IX_PersonnelScheduledActions_event_id");

            migrationBuilder.RenameIndex(
                name: "IX_personnel_scheduled_actions_employee_no",
                table: "PersonnelScheduledActions",
                newName: "IX_PersonnelScheduledActions_employee_no");

            migrationBuilder.RenameIndex(
                name: "IX_personnel_scheduled_actions_effective_date_status",
                table: "PersonnelScheduledActions",
                newName: "IX_PersonnelScheduledActions_effective_date_status");

            migrationBuilder.RenameIndex(
                name: "IX_kafka_event_logs_status",
                table: "KafkaEventLogs",
                newName: "IX_KafkaEventLogs_status");

            migrationBuilder.RenameIndex(
                name: "IX_kafka_event_logs_created_at_utc",
                table: "KafkaEventLogs",
                newName: "IX_KafkaEventLogs_created_at_utc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonnelScheduledActions",
                table: "PersonnelScheduledActions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KafkaEventLogs",
                table: "KafkaEventLogs",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonnelScheduledActions",
                table: "PersonnelScheduledActions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KafkaEventLogs",
                table: "KafkaEventLogs");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "PersonnelScheduledActions",
                newName: "personnel_scheduled_actions");

            migrationBuilder.RenameTable(
                name: "KafkaEventLogs",
                newName: "kafka_event_logs");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_token",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_token");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_employee_no",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_employee_no");

            migrationBuilder.RenameIndex(
                name: "IX_PersonnelScheduledActions_event_id",
                table: "personnel_scheduled_actions",
                newName: "IX_personnel_scheduled_actions_event_id");

            migrationBuilder.RenameIndex(
                name: "IX_PersonnelScheduledActions_employee_no",
                table: "personnel_scheduled_actions",
                newName: "IX_personnel_scheduled_actions_employee_no");

            migrationBuilder.RenameIndex(
                name: "IX_PersonnelScheduledActions_effective_date_status",
                table: "personnel_scheduled_actions",
                newName: "IX_personnel_scheduled_actions_effective_date_status");

            migrationBuilder.RenameIndex(
                name: "IX_KafkaEventLogs_status",
                table: "kafka_event_logs",
                newName: "IX_kafka_event_logs_status");

            migrationBuilder.RenameIndex(
                name: "IX_KafkaEventLogs_created_at_utc",
                table: "kafka_event_logs",
                newName: "IX_kafka_event_logs_created_at_utc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_personnel_scheduled_actions",
                table: "personnel_scheduled_actions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_kafka_event_logs",
                table: "kafka_event_logs",
                column: "id");
        }
    }
}
