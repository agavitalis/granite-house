using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraniteHouse.Models;
using Microsoft.EntityFrameworkCore;
using GraniteHouse.Extensions;

namespace GraniteHouse.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;   
        }

        public async Task<IActionResult> Index()
        {
            var productList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();
            return View(productList);
        }

        public async Task<IActionResult> Details(int Id)
        {
            var product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(m=>m.Id == Id).FirstOrDefaultAsync();

            return View(product);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int Id)
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(listShoppingCart == null)
            {
                listShoppingCart = new List<int>();
            }

            listShoppingCart.Add(Id);
            HttpContext.Session.Set("ssShoppingCart", listShoppingCart);

            return RedirectToAction("Index","Home", new { area = "Customer"});
        }

        
        public async Task<IActionResult> Remove(int Id)
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (listShoppingCart.Count > 0)
            {
                if (listShoppingCart.Contains(Id))
                {
                    listShoppingCart.Remove(Id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", listShoppingCart);

            return RedirectToAction(nameof(Index));
        }



    }
}
