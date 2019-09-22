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
    public class SalesController : Controller
    {
        ApplicationDbContext _context;

        public SalesController()
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
            var client = _context.Clients.SingleOrDefault(c => c.IdentityUserId == userId);

            var itemsToSale = _context.ItemBaskets
                                      .Where(ib => ib.ClientId == client.Id)
                                      .Include(ib => ib.Item)
                                      .ToArray();

            var sale = new Sale();
            sale.Date = DateTime.Now;
            sale.ItemsSumPrice = itemsToSale.Select(ib => ib.Item.CurrentPrice * ib.Count).Sum();
            sale.ExpeditionPrice = 10;
            sale.TotalPrice = sale.ItemsSumPrice + sale.ExpeditionPrice;
            sale.ClientId = client.Id;

            _context.Sales.Add(sale);
            _context.SaveChanges();

            foreach (var itemInBasket in itemsToSale)
            {
                var saleItem = new SaleItem();
                saleItem.Count = itemInBasket.Count;
                saleItem.ItemId = itemInBasket.ItemId;
                saleItem.Price = itemInBasket.Item.CurrentPrice;
                saleItem.SaleId = sale.Id;
                _context.SaleItems.Add(saleItem);
            }
            _context.SaveChanges();

            _context.ItemBaskets.RemoveRange(itemsToSale);
            _context.SaveChanges();

            return View("Confirmation");
        }
    }
}