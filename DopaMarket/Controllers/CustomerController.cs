using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DopaMarket.ViewModels;

namespace DopaMarket.Controllers
{
    [Authorize]
    public class CustomerController : BaseController
    {
        ApplicationDbContext _context;

        public CustomerController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId().ToString();
            var customer = _context.Customers.SingleOrDefault(c => c.IdentityUserId == userId);

            var customerFormViewModel = new CustomerProfileViewModel();
            customerFormViewModel.Customer = customer;

            return View(customerFormViewModel);
        }

        public ActionResult ProfileEdit()
        {
            var userId = User.Identity.GetUserId().ToString();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var customer = _context.Customers.SingleOrDefault(c => c.IdentityUserId == userId);

            var customerProfilViewModel = new CustomerProfileViewModel();
            customerProfilViewModel.Customer  = customer;
            customerProfilViewModel.Email = user.Email;

            return View("ProfileForm", customerProfilViewModel);
        }

        [HttpPost]
        public ActionResult ProfileSave(CustomerProfileViewModel customerFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("CustomerForm", customerFormViewModel);
            }

            var customerInDB = _context.Customers.Single<Customer>(c => c.Id == customerFormViewModel.Customer.Id);
            customerInDB.FirstName = customerFormViewModel.Customer.FirstName;
            customerInDB.LastName = customerFormViewModel.Customer.LastName;
            customerInDB.PhoneNumber = customerFormViewModel.Customer.PhoneNumber;

            _context.SaveChanges();

            return RedirectToAction("ProfileEdit", "Customer");
        }
    }
}