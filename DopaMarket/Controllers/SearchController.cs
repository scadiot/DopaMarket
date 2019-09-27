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

            var itemsRequestPage = itemsRequest.OrderBy(i => i.Name).Skip(ItemPerPage * page).Take(ItemPerPage);

            string urlParameters = (q != string.Empty ? "q=" + q : "");
            urlParameters += (q != string.Empty && c != string.Empty ? "&" : "");
            urlParameters += (c != string.Empty ? "c=" + c : "");

            var searchViewModel = new SearchViewModel();
            searchViewModel.Items = itemsRequestPage.Select(item => new SearchItemViewModel() { Item = item }).ToArray();
            searchViewModel.Query = q;
            searchViewModel.Category = c;
            searchViewModel.UrlParameters = urlParameters;
            searchViewModel.TotalCount = totalItems;
            searchViewModel.PageNumber = page;
            searchViewModel.PageCount = (totalItems / ItemPerPage) + 1;
            searchViewModel.Categories = categories;
            searchViewModel.ChildrenCategories = childrenCategories;

            SetPricesFilters(itemsRequest, ref searchViewModel);

            foreach (var item in searchViewModel.Items)
            {
                item.Image = _context.ItemImages.Where(i => i.ItemId == item.Item.Id).FirstOrDefault();
            }

            return View("Results", searchViewModel);
        }

        void SetPricesFilters(IQueryable<Item> items, ref SearchViewModel searchViewModel)
        {
            searchViewModel.PriceRangeMin = items.Min(i => i.CurrentPrice);
            searchViewModel.PriceRangeStart = searchViewModel.PriceRangeMin;

            searchViewModel.PriceRangeMax = items.Max(i => i.CurrentPrice);
            searchViewModel.PriceRangeEnd = searchViewModel.PriceRangeMax;

            searchViewModel.PriceFilters = SplitPriceRange(items, searchViewModel.PriceRangeMin, searchViewModel.PriceRangeMax, 0);
        }

        PriceFilter[] SplitPriceRange(IQueryable<Item> items, decimal startPrice, decimal endPrice, int depth)
        {
            var result = new List<PriceFilter>();
            decimal middle = startPrice + ((endPrice - startPrice) / 2);
            int range1Count = items.Count(i => i.CurrentPrice >= startPrice && i.CurrentPrice < middle);
            int range2Count = items.Count(i => i.CurrentPrice >= middle && i.CurrentPrice <= endPrice);

            if(range1Count > 0)
            {
                if (depth == 2 || range1Count == 1)
                {
                    var filters1 = new PriceFilter() { PriceStart = startPrice, PriceEnd = middle, Count = range1Count };
                    result.Add(filters1);
                }
                else
                {
                    var filters1 = SplitPriceRange(items, startPrice, middle, depth + 1);
                    result.AddRange(filters1);
                }
            }

            if (range2Count > 0)
            {
                if (depth == 2 || range2Count == 1)
                {
                    var filters2 = new PriceFilter() { PriceStart = middle, PriceEnd = endPrice, Count = range2Count };
                    result.Add(filters2);
                }
                else
                {
                    var filters2 = SplitPriceRange(items, middle, endPrice, depth + 1);
                    result.AddRange(filters2);
                }
            }

            return result.ToArray();
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