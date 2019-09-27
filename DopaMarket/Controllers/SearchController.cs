using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public ActionResult Index(string q, string c, string pf = null, int page = 0)
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
            var pricesFiltersData = SetPricesFilters(itemsRequest, pf);

            var itemsRequestFiltered = itemsRequest;

            itemsRequest = itemsRequest.Where(i => i.CurrentPrice >= pricesFiltersData.PriceRangeStart && i.CurrentPrice <= pricesFiltersData.PriceRangeEnd);

            Expression<Func<Item, bool>> predicate = null;
            foreach (var priceFilters in pricesFiltersData.PriceFilters.Where(f => f.Selected))
            {
                Expression<Func<Item, bool>> subPredicate = i => i.CurrentPrice >= priceFilters.PriceStart && i.CurrentPrice <= priceFilters.PriceEnd;
                if (predicate == null)
                {
                    predicate = subPredicate;
                }
                else
                {
                    predicate = predicate.Or(subPredicate);
                }
            }
            if(predicate != null)
            {
                itemsRequestFiltered = itemsRequestFiltered.Where(predicate);
            }

            int itemsCountAfterFilter = itemsRequestFiltered.Count();

            var itemsRequestPage = itemsRequestFiltered.OrderBy(i => i.Name).Skip(ItemPerPage * page).Take(ItemPerPage);

            var searchViewModel = new SearchViewModel();
            searchViewModel.Query = q;
            searchViewModel.Category = c;
            searchViewModel.PageNumber = page;
            searchViewModel.TotalCount = totalItems;
            searchViewModel.PageCount = (itemsCountAfterFilter / ItemPerPage) + 1;
            searchViewModel.Categories = categories;
            searchViewModel.ChildrenCategories = childrenCategories;
            searchViewModel.PriceRangeMin = pricesFiltersData.PriceRangeMin;
            searchViewModel.PriceRangeMax = pricesFiltersData.PriceRangeMax;
            searchViewModel.PriceRangeStart = pricesFiltersData.PriceRangeStart;
            searchViewModel.PriceRangeEnd = pricesFiltersData.PriceRangeEnd;
            searchViewModel.PriceFilters = pricesFiltersData.PriceFilters;

            searchViewModel.Items = itemsRequestPage.Select(item => new SearchItemViewModel() { Item = item }).ToArray();

            foreach (var item in searchViewModel.Items)
            {
                item.Image = _context.ItemImages.Where(i => i.ItemId == item.Item.Id).FirstOrDefault();
            }

            

            return View("Results", searchViewModel);
        }

        class PricesFiltersData
        {
            public decimal PriceRangeMin { get; set; }
            public decimal PriceRangeStart { get; set; }

            public decimal PriceRangeMax { get; set; }
            public decimal PriceRangeEnd { get; set; }

            public IEnumerable<PriceFilter> PriceFilters { get; set; }
        }

        PricesFiltersData SetPricesFilters(IQueryable<Item> items, string urlParameter)
        {
            PricesFiltersData result = new PricesFiltersData();
            result.PriceRangeMin = items.Min(i => i.CurrentPrice);
            result.PriceRangeStart = result.PriceRangeMin;

            result.PriceRangeMax = items.Max(i => i.CurrentPrice);
            result.PriceRangeEnd = result.PriceRangeMax;

            result.PriceFilters = SplitPriceRange(items, result.PriceRangeMin, result.PriceRangeMax, 0);

            if(urlParameter != null)
            {
                foreach(var parameter in urlParameter.Split('_'))
                {
                    decimal priceStart = decimal.Parse(parameter.Split('-')[0]);
                    decimal priceEnd = decimal.Parse(parameter.Split('-')[1]);
                    result.PriceFilters.Single(pf => pf.PriceStart == priceStart && pf.PriceEnd == priceEnd).Selected = true;
                }
            }
            return result;
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
                    var filters1 = new PriceFilter() { PriceStart = startPrice, PriceEnd = middle, Count = range1Count, Selected = false };
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
                    var filters2 = new PriceFilter() { PriceStart = middle, PriceEnd = endPrice, Count = range2Count, Selected = false };
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