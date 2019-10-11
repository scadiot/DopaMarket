using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DopaMarket.ViewModels;
using System.Data.Entity;
using System.Web.Routing;

namespace DopaMarket.Controllers
{
    [Authorize]
    public class CustomerController : BaseController
    {
        ApplicationDbContext _context;
        Customer _customer;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            _context = new ApplicationDbContext();
            var userId = User.Identity.GetUserId().ToString();
            _customer = _context.Customers.SingleOrDefault(c => c.ApplicationUserId == userId);

            ViewBag.orderCount = _context.Orders.Count(o => o.CustomerId == _customer.Id);
        }

        public ActionResult Index()
        {
            var customerFormViewModel = new CustomerProfileViewModel();
            customerFormViewModel.Customer = _customer;

            return View(customerFormViewModel);
        }

        public ActionResult ProfileEdit()
        {
            var userId = User.Identity.GetUserId().ToString();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            var customerProfilViewModel = new CustomerProfileViewModel();
            customerProfilViewModel.Customer  = _customer;
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

        public ActionResult AddressEdit()
        {
            Address address = null;
            if (_customer.AddressId != null)
            {
                address = _context.Address.SingleOrDefault(a => a.Id == _customer.AddressId);
            }
            else
            {
                address = new Address();
            }

            var customerAddressViewModel = new CustomerAddressViewModel();
            customerAddressViewModel.Address = address;

            return View("AddressForm", customerAddressViewModel);
        }

        [HttpPost]
        public ActionResult AddressSave(CustomerAddressViewModel customerAddressViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("AddressForm", customerAddressViewModel);
            }

            Address addressInDB = null;
            if (_customer.AddressId != null)
            {
                addressInDB = _context.Address.SingleOrDefault(a => a.Id == _customer.AddressId);
            }
            else
            {
                addressInDB = new Address();
                _context.Address.Add(addressInDB);
                _context.SaveChanges();

                _customer.AddressId = addressInDB.Id;
            }

            addressInDB.Street = customerAddressViewModel.Address.Street;
            addressInDB.Street2 = customerAddressViewModel.Address.Street2;
            addressInDB.State = customerAddressViewModel.Address.State;
            addressInDB.PostalCode = customerAddressViewModel.Address.PostalCode;
            addressInDB.State = customerAddressViewModel.Address.State;
            addressInDB.City = customerAddressViewModel.Address.City;
            addressInDB.Country = customerAddressViewModel.Address.Country;
            _context.SaveChanges();

            return RedirectToAction("AddressForm", "Customer");
        }

        public ActionResult Tickets()
        {
            var customerTicketsViewModel = new CustomerTicketsViewModel();
            customerTicketsViewModel.Tickets = _context.Tickets.Where(t => t.CustomerId == _customer.Id).ToArray();

            return View("Tickets", customerTicketsViewModel);
        }

        public ActionResult Orders()
        {
            var costumerOrdersViewModel = new CostumerOrdersViewModel();
            costumerOrdersViewModel.Orders = _context.Orders.Where(o => o.CustomerId == _customer.Id).OrderBy(o => o.Date).ToArray();

            return View("Orders", costumerOrdersViewModel);
        }
    }
}