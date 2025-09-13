using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetStore.Migrations
{
    public partial class AddUserColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [AspNetUsers] ADD [AccountCreated] datetime2 NULL");
            migrationBuilder.Sql("ALTER TABLE [AspNetUsers] ADD [LastLogin] datetime2 NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [AspNetUsers] DROP COLUMN [AccountCreated]");
            migrationBuilder.Sql("ALTER TABLE [AspNetUsers] DROP COLUMN [LastLogin]");
        }
    }
}
