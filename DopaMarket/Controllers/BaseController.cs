using DopaMarket.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ApplicationDbContext _context = new ApplicationDbContext();

            if ( User.Identity.IsAuthenticated )
            {
                var userId = User.Identity.GetUserId().ToString();
                var customer = _context.Customers.SingleOrDefault(c => c.ApplicationUserId == userId);
                ViewBag.customer = customer;
            }

            var categories = _context.Categories.Where(c => c.ParentCategoryId == null).ToArray();
            ViewBag.categories = categories;
            
            base.OnResultExecuting(filterContext);
        }
    }
}