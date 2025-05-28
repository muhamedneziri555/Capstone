using CarpetStore.Migrations;
using CarpetStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CarpetStore.Data
{

    public class CarpetStoreWebDb : IdentityDbContext<ApplicationUser>
    {
        public CarpetStoreWebDb(DbContextOptions<CarpetStoreWebDb> options) : base(options)
        { 
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Acrylic Agra", Detail = "Dimensions: 150x220", Price = 120, IsTrendingProduct = true, ImageUrl = "https://fe3b71.cdn.akinoncloud.com/products/2023/06/02/441/51c2d295-dd5a-4151-ac51-e1ff7752a9b7_size1024x1319.jpg" },
                new Product { Id = 2, Name = "Acrylic Bilbao", Detail = "Dimensions: 150x220", Price = 180, IsTrendingProduct = true, ImageUrl = "https://fe3b71.cdn.akinoncloud.com/products/2023/06/05/678/d5f7b5a8-59f4-4d75-b378-e57676221162_size1024x1319.jpg" },
                new Product { Id = 3, Name = "Acrylic Contour", Detail = "Dimensions: 150x220", Price = 140, IsTrendingProduct = true, ImageUrl = "https://fe3b71.cdn.akinoncloud.com/products/2023/06/02/1154/0a883a2d-25b2-454c-b3c3-01a345f50191_size1024x1319.jpg" },
                new Product { Id = 4, Name = "Acrylic Dulce", Detail = "Dimensions: 150x220", Price = 220, IsTrendingProduct = true, ImageUrl = "https://fe3b71.cdn.akinoncloud.com/products/2023/06/02/1151/d860dcf9-00ca-4a38-8c47-5f6570edae6c_size1024x1319.jpg" },
                new Product { Id = 5, Name = "Acrylic Kids", Detail = "Dimensions: 150x220", Price = 89, IsTrendingProduct = true, ImageUrl = "https://fe3b71.cdn.akinoncloud.com/products/2023/06/02/256/39ba5cc4-6a38-4c7a-8079-ecc3ab2fe929_size1024x1319.jpg" },
                new Product { Id = 6, Name = "Acrylic Ibiza", Detail = "Dimensions: 150x220", Price = 320, IsTrendingProduct = true, ImageUrl = "https://fe3b71.cdn.akinoncloud.com/products/2023/11/20/3382/5ef2b90e-4527-440f-87b0-a84240087ae2_size1024x1319.jpg" },
                new Product { Id = 7, Name = "Acrylic Pena", Detail = "Dimensions: 150x220", Price = 425, IsTrendingProduct = true, ImageUrl = "https://fe3b71.cdn.akinoncloud.com/products/2023/06/05/2620/55e4d03a-d072-4fae-952f-86d89466bbef_size1024x1319.jpg" },
                new Product { Id = 8, Name = "Acrylic Milano", Detail = "Dimensions: 150x220", Price = 280, IsTrendingProduct = true, ImageUrl = "https://fe3b71.cdn.akinoncloud.com/products/2023/06/05/2620/55e4d03a-d072-4fae-952f-86d89466bbef_size1024x1319.jpg" }
            );
        }
    }
}
