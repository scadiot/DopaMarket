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
    public class CustomerController : Controller
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
            var address = _context.Address.SingleOrDefault(c => c.Id == customer.AddressId);

            var customerFormViewModel = new CustomerFormViewModel();
            customerFormViewModel.Customer = customer;
            customerFormViewModel.Address = address;

            return View(customerFormViewModel);
        }

        public ActionResult Edit()
        {
            var userId = User.Identity.GetUserId().ToString();
            var customer = _context.Customers.SingleOrDefault(c => c.IdentityUserId == userId);

            var customerFormViewModel = new CustomerFormViewModel();
            customerFormViewModel.Customer  = customer;
            if (customer.AddressId != null)
                customerFormViewModel.Address = _context.Address.Single(a => a.Id == customer.AddressId);
            else
                customerFormViewModel.Address = new Address();

            return View("CustomerForm", customerFormViewModel);
        }

        [HttpPost]
        public ActionResult Save(CustomerFormViewModel customerFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("CustomerForm", customerFormViewModel);
            }

            Address addressInDB;
            if (customerFormViewModel.Address.Id != 0)
                addressInDB = _context.Address.Single<Address>(a => a.Id == customerFormViewModel.Address.Id);
            else
                addressInDB = new Address();

            addressInDB.Street     = customerFormViewModel.Address.Street;
            addressInDB.Street2    = customerFormViewModel.Address.Street2;
            addressInDB.City       = customerFormViewModel.Address.City;
            addressInDB.State      = customerFormViewModel.Address.State;
            addressInDB.PostalCode = customerFormViewModel.Address.PostalCode;
            addressInDB.Country    = customerFormViewModel.Address.Country;

            _context.Address.Add(addressInDB);
            _context.SaveChanges();

            var customerInDB = _context.Customers.Single<Customer>(c => c.Id == customerFormViewModel.Customer.Id);
            customerInDB.FirstName = customerFormViewModel.Customer.FirstName;
            customerInDB.LastName = customerFormViewModel.Customer.LastName;
            customerInDB.FirstName = customerFormViewModel.Customer.FirstName;
            customerInDB.PhoneNumber = customerFormViewModel.Customer.PhoneNumber;
            customerInDB.AddressId = addressInDB.Id;

            _context.SaveChanges();

            return RedirectToAction("Index", "Customer");
        }
    }
}