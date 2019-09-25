using DopaMarket.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class OrdersController : Controller
    {
        ApplicationDbContext _context;

        public OrdersController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId().ToString();
            var customer = _context.Customers.SingleOrDefault(c => c.IdentityUserId == userId);

            var itemsToOrder = _context.ItemCarts
                                      .Where(ib => ib.CustomerId == customer.Id)
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
    }
}