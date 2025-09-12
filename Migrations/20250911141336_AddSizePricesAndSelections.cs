using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetStore.Migrations
{
    /// <inheritdoc />
    public partial class AddSizePricesAndSelections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectedSize",
                table: "ShoppingCartItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "ShoppingCartItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price120x170",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price150x220",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price200x290",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "OrderDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Price120x170", "Price150x220", "Price200x290" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Price120x170", "Price150x220", "Price200x290" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Price120x170", "Price150x220", "Price200x290" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Price120x170", "Price150x220", "Price200x290" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Price120x170", "Price150x220", "Price200x290" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Price120x170", "Price150x220", "Price200x290" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Price120x170", "Price150x220", "Price200x290" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Price120x170", "Price150x220", "Price200x290" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedSize",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "Price120x170",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price150x220",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price200x290",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "OrderDetail");
        }
    }
}
