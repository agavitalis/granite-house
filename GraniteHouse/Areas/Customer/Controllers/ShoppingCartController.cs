using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };
        }

        //Get Shoping Cart
        public async Task<IActionResult> Index()
        {
            List<int> ListShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (ListShoppingCart.Count > 0)
            {
                foreach (var cartItem in ListShoppingCart)
                {
                    Products prod = _db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == cartItem).FirstOrDefault();
                    ShoppingCartVM.Products.Add(prod);
                }

            }

            return View(ShoppingCartVM);
        }

        //PostShoping Cart (Checkout)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            List<int> ListShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (ListShoppingCart.Count > 0)
            {
                ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate
                                                                .AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour)
                                                                .AddMinutes(ShoppingCartVM.Appointments.AppointmentTime.Minute);
                Appointments appointments = ShoppingCartVM.Appointments;
                _db.Appointments.Add(appointments);
                _db.SaveChanges();

                int appointmentId = appointments.Id;
                foreach (var productId in ListShoppingCart)
                {
                    ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment
                    {
                        AppointmentId = appointmentId,
                        ProductId = productId
                    };

                    _db.ProductsSelectedForAppointment.Add(productsSelectedForAppointment);
                   


                }
                _db.SaveChanges();
                //empty the list and set session of empty list again
                ListShoppingCart = new List<int>();
                HttpContext.Session.Set("ssShoppingCart", ListShoppingCart);


                return RedirectToAction("AppointmentConfirmation","ShoppingCart", new { Id = appointmentId}); 

            }

            return View(ShoppingCartVM);
        }

        public IActionResult Remove(int Id)
        {

            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (listShoppingCart.Count > 0)
            {
                if (listShoppingCart.Contains(Id))
                {
                    listShoppingCart.Remove(Id);
                }
            }

            //set session again   
            HttpContext.Session.Set("ssShoppingCart", listShoppingCart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AppointmentConfirmation(int Id)
        {
            ShoppingCartVM.Appointments = _db.Appointments.Where(a => a.Id == Id).FirstOrDefault();
            List<ProductsSelectedForAppointment> objProdList = _db.ProductsSelectedForAppointment.Where(p => p.AppointmentId == Id).ToList();

            foreach (ProductsSelectedForAppointment prodAptObj in objProdList)
            {
                ShoppingCartVM.Products.Add(_db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).Where(p => p.Id == prodAptObj.ProductId).FirstOrDefault());
            }

            return View(ShoppingCartVM); 
        }
    }
}