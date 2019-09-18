using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class SearchController : Controller
    {
        ApplicationDbContext _context;

        public SearchController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index(string q)
        {
            var keywords = q.ToLower().Split(' ');

            var items = (from i in _context.Items
                         join ik in _context.ItemKeywords on i.Id equals ik.ItemId
                         join k in _context.Keywords on ik.KeywordId equals k.Id
                         where keywords.Contains(k.Word)
                         select new SearchItemViewModel() { Item = i }).ToArray();

            var searchViewModel = new SearchViewModel();
            searchViewModel.Items = items;
            searchViewModel.Search = q;

            foreach(var item in searchViewModel.Items)
            {
                item.Image = _context.ItemImages.Where(i => i.ItemId == item.Item.Id).FirstOrDefault();
            }

            return View("Results", searchViewModel);
        }
    }
}