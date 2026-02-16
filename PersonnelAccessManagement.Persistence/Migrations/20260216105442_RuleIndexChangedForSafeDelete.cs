using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonnelAccessManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RuleIndexChangedForSafeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 DROP INDEX IF EXISTS ux_rules_scope;

                                 CREATE UNIQUE INDEX ux_rules_scope
                                 ON "Rules" (COALESCE("Campus", '*'), COALESCE("Title", '*'))
                                 WHERE "IsDeleted" = false;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 DROP INDEX IF EXISTS ux_rules_scope;

                                 CREATE UNIQUE INDEX ux_rules_scope
                                 ON "Rules" (COALESCE("Campus", '*'), COALESCE("Title", '*'));
                                 """);
        }
    }
}
