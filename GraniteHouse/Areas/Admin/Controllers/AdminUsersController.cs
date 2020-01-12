using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Models;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouse.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            //get all the users of this application
            return View(_db.ApplicationUsers.ToList());
        }

        //Get User
        public async Task<IActionResult> Edit(string Id)
        {
            if(Id == null || Id.Trim().Length == 0)
            {
                return NotFound();
            }
            else
            {
                var userFromDB = await _db.ApplicationUsers.FindAsync(Id);

                if (userFromDB == null)
                {
                    return NotFound();
                }

                return View(userFromDB);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string Id, ApplicationUser applicationUser)
        {
            if (Id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == Id).FirstOrDefault();
                userFromDb.Name = applicationUser.Name;
                userFromDb.PhoneNumber = applicationUser.PhoneNumber;

                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(applicationUser);
        }

        //Get Delete User
        public async Task<IActionResult> Delete(string Id)
        {
            if (Id == null || Id.Trim().Length == 0)
            {
                return NotFound();
            }
            else
            {
                var userFromDB = await _db.ApplicationUsers.FindAsync(Id);

                if (userFromDB == null)
                {
                    return NotFound();
                }

                return View(userFromDB);
            }

        }

        //Post Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string Id)
        {
            
            ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == Id).FirstOrDefault();
            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        
        }
    }
}