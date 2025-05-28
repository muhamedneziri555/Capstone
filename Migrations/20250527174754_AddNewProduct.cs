using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetStore.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Detail", "ImageUrl", "IsTrendingProduct", "Name", "Price" },
                values: new object[] { 8, "Dimensions: 150x220", "https://fe3b71.cdn.akinoncloud.com/products/2023/06/05/2620/55e4d03a-d072-4fae-952f-86d89466bbef_size1024x1319.jpg", true, "Acrylic Milano", 280m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
