using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetStore.Migrations
{
    public partial class AddUserColumnsOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[AspNetUsers]') AND name = 'AccountCreated')
                BEGIN
                    ALTER TABLE [AspNetUsers] ADD [AccountCreated] datetime2 NULL
                END
            ");
            
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[AspNetUsers]') AND name = 'LastLogin')
                BEGIN
                    ALTER TABLE [AspNetUsers] ADD [LastLogin] datetime2 NULL
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [AspNetUsers] DROP COLUMN [AccountCreated]");
            migrationBuilder.Sql("ALTER TABLE [AspNetUsers] DROP COLUMN [LastLogin]");
        }
    }
}
