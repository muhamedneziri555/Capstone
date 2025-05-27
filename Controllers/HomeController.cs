using CarpetStore.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CarpetStore.Controllers
{
    public class HomeController : Controller
    {
        private IProductRepository productRepository;
        public HomeController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        public IActionResult Index()
        {

            return View(productRepository.GetTrendingProducts());
        }
    }
}

