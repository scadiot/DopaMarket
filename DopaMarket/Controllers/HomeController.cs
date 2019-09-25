using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class HomeController : BaseController
    {
        ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var homeViewModel = new HomeViewModel();
            homeViewModel.Categories = _context.Categories.Where(c => c.ParentCategoryId == null).ToArray();
            homeViewModel.Items = _context.Items
                                          .OrderBy(d => d.InsertDate)
                                          .Take(20)
                                          .Select(i => new SearchItemViewModel() { Item = i })
                                          .ToArray();
            return View("index", homeViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}