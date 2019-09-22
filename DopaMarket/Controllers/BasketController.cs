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
    public class BasketController : Controller
    {
        ApplicationDbContext _context;

        public BasketController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId().ToString();
            var client = _context.Clients.SingleOrDefault(c => c.IdentityUserId == userId);

            var items = (from i in _context.Items
                         join ib in _context.ItemBaskets on i.Id equals ib.ItemId
                         where ib.ClientId == client.Id
                         select new BasketItemViewModel() { Item = i, Quantity = ib.Count }).ToArray();

            var basketViewModel = new BasketViewModel();
            basketViewModel.Items = items;

            return View(basketViewModel);
        }

        public JsonResult AddItem(int id)
        {
            var userId = User.Identity.GetUserId().ToString();
            var client = _context.Clients.SingleOrDefault(c => c.IdentityUserId == userId);

            if(_context.ItemBaskets.Any(ib => ib.ClientId == client.Id && ib.ItemId == id))
            {
                return Json(new { result = "error", message = "already exist" }, JsonRequestBehavior.AllowGet);
            }

            var itemBasket = new ItemBasket();
            itemBasket.ItemId = id;
            itemBasket.ClientId = client.Id;
            itemBasket.Count = 1;
            _context.ItemBaskets.Add(itemBasket);
            _context.SaveChanges();

            return Json(new { result = "added" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveItem(int id)
        {
            var userId = User.Identity.GetUserId().ToString();
            var client = _context.Clients.SingleOrDefault(c => c.IdentityUserId == userId);

            var itemInBasket = _context.ItemBaskets.SingleOrDefault(ib => ib.ClientId == client.Id && ib.ItemId == id);
            if (itemInBasket == null)
            {
                return Json(new { result = "error", message = "not in bascket" }, JsonRequestBehavior.AllowGet);
            }

            _context.ItemBaskets.Remove(itemInBasket);
            _context.SaveChanges();

            return Json(new { result = "removed" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeCountItem(int id, int count)
        {
            var userId = User.Identity.GetUserId().ToString();
            var client = _context.Clients.SingleOrDefault(c => c.IdentityUserId == userId);

            var itemInBasket = _context.ItemBaskets.SingleOrDefault(ib => ib.ClientId == client.Id && ib.ItemId == id);
            if (itemInBasket == null)
            {
                return Json(new { result = "error", message = "not in bascket" }, JsonRequestBehavior.AllowGet);
            }

            if (count < 1)
            {
                return Json(new { result = "error", message = "invalid request" }, JsonRequestBehavior.AllowGet);
            }

            itemInBasket.Count = count;
            _context.SaveChanges();

            return Json(new { result = "count_changed", count = count }, JsonRequestBehavior.AllowGet);
        }
    }
}