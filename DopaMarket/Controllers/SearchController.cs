using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class SearchController : BaseController
    {
        const int ItemPerPage = 3;

        ApplicationDbContext _context;

        public SearchController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index(string q, string c, int page = 0)
        {
            q = q == null ? string.Empty : q.Trim();
            c = c == null ? string.Empty : c.Trim();

            Category category = null;
            if (c != string.Empty)
            {
                category = _context.Categories.SingleOrDefault(cat => cat.LinkName == c);
            }

            Category[] childrenCategories = null;
            if(c != string.Empty && q == string.Empty)
            {
                childrenCategories = _context.Categories
                                             .Where(cat => cat.ParentCategoryId == category.Id)
                                             .ToArray();
            }


            IQueryable<Item> itemsRequest = _context.Items;
            if (q.Trim() != string.Empty)
            {
                var keywords = q.ToLower().Split(' ');

                itemsRequest = from item in itemsRequest
                               join itemKeyword in _context.ItemKeywords on item.Id equals itemKeyword.ItemId
                               join keyword in _context.Keywords on itemKeyword.KeywordId equals keyword.Id
                               where keywords.Contains(keyword.Word)
                               select item;
            }

            if (c.Trim() != string.Empty)
            { 
                itemsRequest = from item in itemsRequest
                               join ItemCategory in _context.ItemCategories on item.Id equals ItemCategory.ItemId
                               join cat in _context.Categories on ItemCategory.CategoryId equals category.Id
                               where cat.LinkName == c
                               select item;
            }

            int totalItems = itemsRequest.Count();
            var categories = GetCategories(itemsRequest);

            itemsRequest = itemsRequest.OrderBy(i => i.Name).Skip(ItemPerPage * page).Take(ItemPerPage);

            string urlParameters = (q != string.Empty ? "q=" + q : "");
            urlParameters += (q != string.Empty && c != string.Empty ? "&" : "");
            urlParameters += (c != string.Empty ? "c=" + c : "");

            var searchViewModel = new SearchViewModel();
            searchViewModel.Items = itemsRequest.Select(item => new SearchItemViewModel() { Item = item }).ToArray();
            searchViewModel.Query = q;
            searchViewModel.Category = c;
            searchViewModel.UrlParameters = urlParameters;
            searchViewModel.TotalCount = totalItems;
            searchViewModel.PageNumber = page;
            searchViewModel.PageCount = (totalItems / ItemPerPage) + 1;
            searchViewModel.Categories = categories;
            searchViewModel.ChildrenCategories = childrenCategories;

            foreach (var item in searchViewModel.Items)
            {
                item.Image = _context.ItemImages.Where(i => i.ItemId == item.Item.Id).FirstOrDefault();
            }

            return View("Results", searchViewModel);
        }

        CategoryViewModel[] GetCategories(IQueryable<Item> items)
        {
            var categories = _context.Categories.Where(c => c.ParentCategoryId == null).ToArray();
            var categoriesIds = categories.Select(cat => cat.Id).ToArray();
            var children = _context.Categories.Where(c => categoriesIds.Contains((int)c.ParentCategoryId)).ToArray();

            var CategoryViewModels = new List<CategoryViewModel>();
            foreach (var category in categories)
            {
                var categoryViewModel = new CategoryViewModel();
                categoryViewModel.Category = category;

                categoryViewModel.Children = children.Where(c => c.ParentCategoryId == category.Id)
                                                     .Select(c => new CategoryViewModel() { Category = c})
                                                     .ToArray();

                CategoryViewModels.Add(categoryViewModel);
            }

            return CategoryViewModels.ToArray();
        }
    }
}