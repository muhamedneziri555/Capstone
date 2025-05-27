using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarpetStore.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Detail", "ImageUrl", "IsTrendingProduct", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Dimensions: 150x220", "https://fe3b71.cdn.akinoncloud.com/products/2023/06/02/441/51c2d295-dd5a-4151-ac51-e1ff7752a9b7_size1024x1319.jpg", true, "Agra", 120m },
                    { 2, "Dimensions: 150x220", "https://fe3b71.cdn.akinoncloud.com/products/2023/06/05/678/d5f7b5a8-59f4-4d75-b378-e57676221162_size1024x1319.jpg", true, "Bilbao", 180m },
                    { 3, "Dimensions: 150x220", "https://fe3b71.cdn.akinoncloud.com/products/2023/06/02/1154/0a883a2d-25b2-454c-b3c3-01a345f50191_size1024x1319.jpg", true, "Contour", 140m },
                    { 4, "Dimensions: 150x220", "https://fe3b71.cdn.akinoncloud.com/products/2023/06/02/1151/d860dcf9-00ca-4a38-8c47-5f6570edae6c_size1024x1319.jpg", true, "Dulce", 220m },
                    { 5, "Dimensions: 150x220", "https://fe3b71.cdn.akinoncloud.com/products/2023/06/02/256/39ba5cc4-6a38-4c7a-8079-ecc3ab2fe929_size1024x1319.jpg", true, "Kids", 89m },
                    { 6, "Dimensions: 150x220", "https://fe3b71.cdn.akinoncloud.com/products/2023/11/20/3382/5ef2b90e-4527-440f-87b0-a84240087ae2_size1024x1319.jpg", true, "Ibiza", 320m },
                    { 7, "Dimensions: 150x220", "https://fe3b71.cdn.akinoncloud.com/products/2023/06/05/2620/55e4d03a-d072-4fae-952f-86d89466bbef_size1024x1319.jpg", true, "Pena", 425m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
