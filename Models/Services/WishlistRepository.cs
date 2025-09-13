using CarpetStore.Data;
using CarpetStore.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarpetStore.Models.Services
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly CarpetStoreWebDb _dbContext;

        public WishlistRepository(CarpetStoreWebDb dbContext)
        {
            _dbContext = dbContext;
        }

        public List<WishlistItem> GetWishlistItems(string userId)
        {
            return _dbContext.WishlistItems
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .OrderByDescending(w => w.DateAdded)
                .ToList();
        }

        public void AddToWishlist(string userId, int productId)
        {
            // Check if item already exists in wishlist
            var existingItem = _dbContext.WishlistItems
                .FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);

            if (existingItem == null)
            {
                var wishlistItem = new WishlistItem
                {
                    UserId = userId,
                    ProductId = productId,
                    DateAdded = DateTime.Now
                };

                _dbContext.WishlistItems.Add(wishlistItem);
                _dbContext.SaveChanges();
            }
        }

        public void RemoveFromWishlist(string userId, int productId)
        {
            var wishlistItem = _dbContext.WishlistItems
                .FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem != null)
            {
                _dbContext.WishlistItems.Remove(wishlistItem);
                _dbContext.SaveChanges();
            }
        }

        public bool IsInWishlist(string userId, int productId)
        {
            return _dbContext.WishlistItems
                .Any(w => w.UserId == userId && w.ProductId == productId);
        }

        public int GetWishlistCount(string userId)
        {
            return _dbContext.WishlistItems
                .Count(w => w.UserId == userId);
        }
    }
}








