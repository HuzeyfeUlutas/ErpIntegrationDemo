using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PersonnelAccessManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddKafkaEventAndPersonnelSchedulesJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kafka_event_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    topic = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    partition_no = table.Column<int>(type: "integer", nullable: false),
                    offset = table.Column<long>(type: "bigint", nullable: false),
                    message_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    message_value = table.Column<string>(type: "text", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: false),
                    error_stack_trace = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "FAILED"),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    retry_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kafka_event_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "personnel_scheduled_actions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    action_type = table.Column<int>(type: "integer", nullable: false),
                    effective_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personnel_scheduled_actions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kafka_event_logs_created_at_utc",
                table: "kafka_event_logs",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_kafka_event_logs_status",
                table: "kafka_event_logs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_personnel_scheduled_actions_effective_date_status",
                table: "personnel_scheduled_actions",
                columns: new[] { "effective_date", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_personnel_scheduled_actions_employee_no",
                table: "personnel_scheduled_actions",
                column: "employee_no");

            migrationBuilder.CreateIndex(
                name: "IX_personnel_scheduled_actions_event_id",
                table: "personnel_scheduled_actions",
                column: "event_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kafka_event_logs");

            migrationBuilder.DropTable(
                name: "personnel_scheduled_actions");
        }
    }
}
