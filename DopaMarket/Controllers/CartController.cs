using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DopaMarket.ViewModels;
using System.Net;

namespace DopaMarket.Controllers
{
    public class CartController : BaseController
    {
        ApplicationDbContext _context;

        public CartController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var items = (from i in _context.Items
                         join ib in _context.ItemCarts on i.Id equals ib.ItemId
                         where ib.SessionId == Session.SessionID
                         select new CartItemViewModel() { Item = i, Quantity = ib.Count }).ToArray();

            var cartViewModel = new CartViewModel();
            cartViewModel.Items = items;
            cartViewModel.SubTotal = cartViewModel.Items.Select(i => i.Item.CurrentPrice * i.Quantity).Sum();

            return View(cartViewModel);
        }

        public JsonResult AddItem(int id, int count)
        {
            if(_context.ItemCarts.Any(ib => ib.SessionId == Session.SessionID && ib.ItemId == id))
            {
                return Json(new { result = "error", message = "already exist" }, JsonRequestBehavior.AllowGet);
            }

            var itemCart = new ItemCart();
            itemCart.ItemId = id;
            itemCart.SessionId = Session.SessionID;
            itemCart.Count = count;
            _context.ItemCarts.Add(itemCart);
            _context.SaveChanges();

            return Json(new { result = "added" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveItem(int id)
        {
            var userId = User.Identity.GetUserId().ToString();
            var customer = _context.Customers.SingleOrDefault(c => c.IdentityUserId == userId);

            var itemInCart = _context.ItemCarts.SingleOrDefault(ib => ib.SessionId == Session.SessionID && ib.ItemId == id);
            if (itemInCart == null)
            {
                return Json(new { result = "error", message = "not in cart" }, JsonRequestBehavior.AllowGet);
            }

            _context.ItemCarts.Remove(itemInCart);
            _context.SaveChanges();

            return Json(new { result = "removed" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeCountItem(int id, int count)
        {
                var userId = User.Identity.GetUserId().ToString();
                var customer = _context.Customers.SingleOrDefault(c => c.IdentityUserId == userId);

            var itemInCart = _context.ItemCarts.SingleOrDefault(ib => ib.SessionId == Session.SessionID && ib.ItemId == id);
            if (itemInCart == null)
            {
                return Json(new { result = "error", message = "not in cart" }, JsonRequestBehavior.AllowGet);
            }

            if (count < 1)
            {
                return Json(new { result = "error", message = "invalid request" }, JsonRequestBehavior.AllowGet);
            }

            itemInCart.Count = count;
            _context.SaveChanges();

            return Json(new { result = "count_changed", count = count }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListItems()
        {
            if(!User.Identity.IsAuthenticated)
            {              
                return Json( new { error = "authentication required" }, JsonRequestBehavior.AllowGet);
            }

            var userId = User.Identity.GetUserId().ToString();
            var customer = _context.Customers.SingleOrDefault(c => c.IdentityUserId == userId);

            var items = (from i in _context.Items
                         join ib in _context.ItemCarts on i.Id equals ib.ItemId
                         where ib.SessionId == Session.SessionID
                         select new CartItemViewModel() { Item = i, Quantity = ib.Count }).ToArray();

            var cartViewModel = new CartViewModel();
            cartViewModel.Items = items;

            return Json(cartViewModel, JsonRequestBehavior.AllowGet);
        }
    }
}