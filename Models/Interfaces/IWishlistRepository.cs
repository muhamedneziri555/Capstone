using CarpetStore.Models;

namespace CarpetStore.Models.Interfaces
{
    public interface IWishlistRepository
    {
        List<WishlistItem> GetWishlistItems(string userId);
        void AddToWishlist(string userId, int productId);
        void RemoveFromWishlist(string userId, int productId);
        bool IsInWishlist(string userId, int productId);
        int GetWishlistCount(string userId);
    }
}








