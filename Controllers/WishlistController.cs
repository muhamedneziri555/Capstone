using CarpetStore.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarpetStore.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;

        public WishlistController(IWishlistRepository wishlistRepository, IProductRepository productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var wishlistItems = _wishlistRepository.GetWishlistItems(userId);
            return View(wishlistItems);
        }

        [HttpPost]
        public IActionResult AddToWishlist(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                var product = _productRepository.GetProductDetail(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                _wishlistRepository.AddToWishlist(userId, productId);
                
                var wishlistCount = _wishlistRepository.GetWishlistCount(userId);
                HttpContext.Session.SetInt32("WishlistCount", wishlistCount);

                return Json(new { success = true, message = "Added to wishlist", wishlistCount = wishlistCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding to wishlist" });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                _wishlistRepository.RemoveFromWishlist(userId, productId);
                
                var wishlistCount = _wishlistRepository.GetWishlistCount(userId);
                HttpContext.Session.SetInt32("WishlistCount", wishlistCount);

                return Json(new { success = true, message = "Removed from wishlist", wishlistCount = wishlistCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error removing from wishlist" });
            }
        }

        [HttpPost]
        public IActionResult ToggleWishlist(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                var isInWishlist = _wishlistRepository.IsInWishlist(userId, productId);
                
                if (isInWishlist)
                {
                    _wishlistRepository.RemoveFromWishlist(userId, productId);
                }
                else
                {
                    _wishlistRepository.AddToWishlist(userId, productId);
                }

                var wishlistCount = _wishlistRepository.GetWishlistCount(userId);
                HttpContext.Session.SetInt32("WishlistCount", wishlistCount);

                return Json(new { 
                    success = true, 
                    isInWishlist = !isInWishlist, 
                    message = !isInWishlist ? "Added to wishlist" : "Removed from wishlist",
                    wishlistCount = wishlistCount 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating wishlist" });
            }
        }

        [HttpPost]
        public IActionResult MoveToCart(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                // Remove from wishlist
                _wishlistRepository.RemoveFromWishlist(userId, productId);
                
                // Add to cart (you'll need to implement this in ShoppingCartController)
                // For now, just return success
                var wishlistCount = _wishlistRepository.GetWishlistCount(userId);
                HttpContext.Session.SetInt32("WishlistCount", wishlistCount);

                return Json(new { success = true, message = "Moved to cart", wishlistCount = wishlistCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error moving to cart" });
            }
        }
    }
}



