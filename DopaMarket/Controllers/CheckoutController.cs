using DopaMarket.Models;
using DopaMarket.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class CheckoutController : BaseController
    {
        ApplicationDbContext _context;

        public CheckoutController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId().ToString();
            var customer = _context.Customers.SingleOrDefault(c => c.ApplicationUserId == userId);

            var costumerOrdersViewModel = new CostumerOrdersViewModel();
            costumerOrdersViewModel.Orders = _context.Orders.Where(o => o.CustomerId == customer.Id).OrderBy(o => o.Date).ToArray();

            return View("Index", costumerOrdersViewModel);
        }

        public ActionResult Address()
        {
            return View("Address", getCheckoutViewModel());
        }

        public ActionResult AddressSave(CheckoutViewModel checkoutViewModel)
        {
            Session.Add("BillingAddress.FirstName", checkoutViewModel.BillingAddress.FirstName);
            Session.Add("BillingAddress.LastName", checkoutViewModel.BillingAddress.LastName);
            Session.Add("BillingAddress.Phone", checkoutViewModel.BillingAddress.Phone);
            Session.Add("BillingAddress.Email", checkoutViewModel.BillingAddress.Email);
            Session.Add("BillingAddress.Company", checkoutViewModel.BillingAddress.Company);
            Session.Add("BillingAddress.Street", checkoutViewModel.BillingAddress.Street);
            Session.Add("BillingAddress.Street2", checkoutViewModel.BillingAddress.Street2);
            Session.Add("BillingAddress.State", checkoutViewModel.BillingAddress.State);
            Session.Add("BillingAddress.PostalCode", checkoutViewModel.BillingAddress.PostalCode);
            Session.Add("BillingAddress.Country", checkoutViewModel.BillingAddress.Country);
            Session.Add("BillingAddress.City", checkoutViewModel.BillingAddress.City);

            if(checkoutViewModel.ShippingAddress != null)
            {
                Session.Add("ShippingAddress.FirstName", checkoutViewModel.ShippingAddress.FirstName);
                Session.Add("ShippingAddress.LastName", checkoutViewModel.ShippingAddress.LastName);
                Session.Add("ShippingAddress.Phone", checkoutViewModel.ShippingAddress.Phone);
                Session.Add("ShippingAddress.Email", checkoutViewModel.ShippingAddress.Email);
                Session.Add("ShippingAddress.Company", checkoutViewModel.ShippingAddress.Company);
                Session.Add("ShippingAddress.Street", checkoutViewModel.ShippingAddress.Street);
                Session.Add("ShippingAddress.Street2", checkoutViewModel.ShippingAddress.Street2);
                Session.Add("ShippingAddress.State", checkoutViewModel.ShippingAddress.State);
                Session.Add("ShippingAddress.PostalCode", checkoutViewModel.ShippingAddress.PostalCode);
                Session.Add("ShippingAddress.Country", checkoutViewModel.ShippingAddress.Country);
                Session.Add("ShippingAddress.City", checkoutViewModel.ShippingAddress.City);
            }
            
            return RedirectToAction("Shipping", "Checkout");
        }

        public ActionResult Shipping()
        {
            return View("Shipping", getCheckoutViewModel());
        }

        public ActionResult ShippingSave()
        {
            return RedirectToAction("Payment", "Checkout");
        }

        public ActionResult Payment()
        {
            return View("Payment", getCheckoutViewModel());
        }


        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId().ToString();
            var customer = _context.Customers.SingleOrDefault(c => c.ApplicationUserId == userId);

            var itemsToOrder = _context.ItemCarts
                                      .Where(ib => ib.SessionId == Session.SessionID)
                                      .Include(ib => ib.Item)
                                      .ToArray();

            var order = new Order();
            order.Date = DateTime.Now;
            order.ItemsSumPrice = itemsToOrder.Select(ib => ib.Item.CurrentPrice * ib.Count).Sum();
            order.ExpeditionPrice = 10;
            order.TotalPrice = order.ItemsSumPrice + order.ExpeditionPrice;
            order.CustomerId = customer.Id;

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var itemInCart in itemsToOrder)
            {
                var orderItem = new OrderItem();
                orderItem.Count = itemInCart.Count;
                orderItem.ItemId = itemInCart.ItemId;
                orderItem.Price = itemInCart.Item.CurrentPrice;
                orderItem.OrderId = order.Id;
                _context.OrderItems.Add(orderItem);
            }
            _context.SaveChanges();

            _context.ItemCarts.RemoveRange(itemsToOrder);
            _context.SaveChanges();

            return View("Confirmation");
        }

        CheckoutViewModel getCheckoutViewModel()
        {
            var userId = User.Identity.GetUserId().ToString();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var customer = _context.Customers.SingleOrDefault(c => c.ApplicationUserId == userId);

            var address = _context.Address.SingleOrDefault(a => a.Id == customer.AddressId);

            var result = new CheckoutViewModel();

            result.BillingAddress = new OrderAddress();
            result.BillingAddress.FirstName = Session["BillingAddress.FirstName"] != null ? Session["BillingAddress.FirstName"].ToString() : customer.FirstName;
            result.BillingAddress.LastName = Session["BillingAddress.LastName"] != null ? Session["BillingAddress.LastName"].ToString() : customer.LastName;
            result.BillingAddress.Email = Session["BillingAddress.Email"] != null ? Session["BillingAddress.Email"].ToString() : user.Email;
            result.BillingAddress.Phone = Session["BillingAddress.Phone"] != null ? Session["BillingAddress.Phone"].ToString() : user.PhoneNumber;
            result.BillingAddress.Company = Session["BillingAddress.Company"] != null ? Session["BillingAddress.Company"].ToString() : "";
            result.BillingAddress.Street = Session["BillingAddress.Street"] != null ? Session["BillingAddress.Street"].ToString() : address.Street;
            result.BillingAddress.Street2 = Session["BillingAddress.Street2"] != null ? Session["BillingAddress.Street2"].ToString() : address.Street2;
            result.BillingAddress.State = Session["BillingAddress.State"] != null ? Session["BillingAddress.State"].ToString() : address.State;
            result.BillingAddress.PostalCode = Session["BillingAddress.PostalCode"] != null ? Session["BillingAddress.PostalCode"].ToString() : address.PostalCode;
            result.BillingAddress.Country = Session["BillingAddress.Country"] != null ? Session["BillingAddress.Country"].ToString() : address.Country;
            result.BillingAddress.City = Session["BillingAddress.City"] != null ? Session["BillingAddress.City"].ToString() : address.City;

            result.ShippingAddress = new OrderAddress();
            result.ShippingAddress.FirstName = Session["ShippingAddress.FirstName"] != null ? Session["ShippingAddress.FirstName"].ToString() : customer.FirstName;
            result.ShippingAddress.LastName = Session["ShippingAddress.LastName"] != null ? Session["ShippingAddress.LastName"].ToString() : customer.LastName;
            result.ShippingAddress.Email = Session["ShippingAddress.Email"] != null ? Session["ShippingAddress.Email"].ToString() : user.Email;
            result.ShippingAddress.Phone = Session["ShippingAddress.Phone"] != null ? Session["ShippingAddress.Phone"].ToString() : user.PhoneNumber;
            result.ShippingAddress.Company = Session["ShippingAddress.Company"] != null ? Session["ShippingAddress.Company"].ToString() : "";
            result.ShippingAddress.Street = Session["ShippingAddress.Street"] != null ? Session["ShippingAddress.Street"].ToString() : address.Street;
            result.ShippingAddress.Street2 = Session["ShippingAddress.Street2"] != null ? Session["ShippingAddress.Street2"].ToString() : address.Street2;
            result.ShippingAddress.State = Session["ShippingAddress.State"] != null ? Session["ShippingAddress.State"].ToString() : address.State;
            result.ShippingAddress.PostalCode = Session["ShippingAddress.PostalCode"] != null ? Session["ShippingAddress.PostalCode"].ToString() : address.PostalCode;
            result.ShippingAddress.Country = Session["ShippingAddress.Country"] != null ? Session["ShippingAddress.Country"].ToString() : address.Country;
            result.ShippingAddress.City = Session["ShippingAddress.City"] != null ? Session["ShippingAddress.City"].ToString() : address.City;

            return result;
        }
    }
}