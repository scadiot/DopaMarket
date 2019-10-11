using DopaMarket.Models;
using DopaMarket.ViewModels;
using LinqKit;
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
        class RequestInfo
        {
            //Input data
            public Category Category { get; set; }
            public string KeywordsString { get; set; }
            public IEnumerable<string> Keywords { get; set; }
            public IEnumerable<Brand> SelectedBrands { get; set; }

            public decimal PriceRangeMin { get; set; }
            public decimal PriceRangeStart { get; set; }

            public decimal PriceRangeMax { get; set; }
            public decimal PriceRangeEnd { get; set; }

            public IEnumerable<PriceFilter> PriceFilters { get; set; }
            public IEnumerable<SearchBrandViewModel> Brands { get; set; }

            public string Sort { get; set; }

            //Output data
            public IQueryable<Item> ItemsRequest { get; set; }
            public IQueryable<Item> ItemsRequestAfterFilter { get; set; }
            public Item[] PageItems { get; set; }
            public int ItemCountTotal { get; set; }
            public int ItemCountAfterFilter { get; set; }
            public int PageCount { get; set; }
            public int PageNumber { get; set; }
            
        }

        const int ItemPerPage = 3;

        ApplicationDbContext _context;

        public SearchController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index(string q, string c, string pf = null, decimal? minPrice = null, decimal? maxPrice = null, string b = "", string sort = "", int page = 0)
        {
            var requestInfo = new RequestInfo();
            GetCategory(ref requestInfo, c);
            ParseKeywords(ref requestInfo, q);

            requestInfo.ItemsRequest = _context.Items;
            CreateRequestByKeywords(ref requestInfo);
            CreateRequestByCategory(ref requestInfo);

            requestInfo.ItemCountTotal = requestInfo.ItemsRequest.Count();
            requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequest;

            ParsePriceRange(ref requestInfo, minPrice, maxPrice);
            ParsePricesFilters(ref requestInfo, pf);
            ParseBrandsFilters(ref requestInfo, b);

            SortRequest(ref requestInfo, sort);
            FinalizeRequest(ref requestInfo, page);

            var searchViewModel = new SearchViewModel();
            searchViewModel.Query = requestInfo.KeywordsString;
            searchViewModel.Category = requestInfo.Category != null ? requestInfo.Category.LinkName : "";
            searchViewModel.Sort = requestInfo.Sort;
            searchViewModel.PageNumber = requestInfo.PageNumber;
            searchViewModel.TotalCount = requestInfo.ItemCountTotal;
            searchViewModel.PageCount = requestInfo.PageCount;
            searchViewModel.Categories = GetCategories();
            searchViewModel.PriceRangeMin = requestInfo.PriceRangeMin;
            searchViewModel.PriceRangeMax = requestInfo.PriceRangeMax;
            searchViewModel.PriceRangeStart = requestInfo.PriceRangeStart;
            searchViewModel.PriceRangeEnd = requestInfo.PriceRangeEnd;
            searchViewModel.PriceFilters = requestInfo.PriceFilters;
            searchViewModel.Items = requestInfo.PageItems.Select(item => new SearchItemViewModel() { Item = item }).ToArray();
            searchViewModel.Brands = requestInfo.Brands;

            return View("Results", searchViewModel);
        }

        void GetCategory(ref RequestInfo requestInfo, string categoryLinkName)
        {
            categoryLinkName = categoryLinkName == null ? string.Empty : categoryLinkName.Trim();

            if (categoryLinkName != string.Empty)
            {
                requestInfo.Category = _context.Categories.SingleOrDefault(cat => cat.LinkName == categoryLinkName);
            }
        }

        void ParseKeywords(ref RequestInfo requestInfo, string keywordsString)
        {
            if (keywordsString == null || keywordsString.Trim() == "")
            {
                requestInfo.Keywords = new string[0];
                return;
            }

            keywordsString = keywordsString.ToLower().Trim();

            requestInfo.KeywordsString = keywordsString;
            keywordsString = keywordsString == null ? string.Empty : keywordsString.Trim();
            requestInfo.Keywords = keywordsString.ToLower().Split(' ');
        }

        void CreateRequestByKeywords(ref RequestInfo requestInfo)
        {
            if(requestInfo.Keywords.Count() == 0)
            {
                return;
            }

            var keywords = requestInfo.Keywords;
            requestInfo.ItemsRequest = from item in requestInfo.ItemsRequest
                                       join itemKeyword in _context.ItemKeywords on item.Id equals itemKeyword.ItemId
                                       join keyword in _context.Keywords on itemKeyword.KeywordId equals keyword.Id
                                       where keywords.Contains(keyword.Word)
                                       group item by item into g
                                       where g.Count() == keywords.Count()
                                       select g.Key;
        }

        void CreateRequestByCategory(ref RequestInfo requestInfo)
        {
            if (requestInfo.Category == null)
            {
                return;
            }
            int i = requestInfo.ItemsRequest.Count();

            var category = requestInfo.Category;
            requestInfo.ItemsRequest = from item in requestInfo.ItemsRequest
                                       join ItemCategory in _context.ItemCategories on item.Id equals ItemCategory.ItemId
                                       where ItemCategory.CategoryId == category.Id
                                       select item;

            
        }

        void ParsePriceRange(ref RequestInfo requestInfo, decimal? urlParameterStart, decimal? urlParameterEnd)
        {
            if(requestInfo.ItemsRequest.Count() == 0)
            {
                return;
            }

            requestInfo.PriceRangeMin = requestInfo.ItemsRequest.Min(i => i.CurrentPrice);
            requestInfo.PriceRangeStart = urlParameterStart != null ? (decimal)urlParameterStart : requestInfo.PriceRangeMin;

            requestInfo.PriceRangeMax = requestInfo.ItemsRequest.Max(i => i.CurrentPrice);
            requestInfo.PriceRangeEnd = urlParameterEnd != null ? (decimal)urlParameterEnd : requestInfo.PriceRangeMax;

            if (urlParameterStart == null || urlParameterEnd == null)
            {
                return;
            }

            decimal priceRangeStart = requestInfo.PriceRangeStart;
            decimal priceRangeEnd = requestInfo.PriceRangeEnd;
            requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.Where(i => i.CurrentPrice >= priceRangeStart && i.CurrentPrice <= priceRangeEnd);
        }

        void ParsePricesFilters(ref RequestInfo requestInfo, string urlParameter)
        {
            requestInfo.PriceFilters = SplitPriceRange(requestInfo.ItemsRequest, requestInfo.PriceRangeMin, requestInfo.PriceRangeMax, 0);

            if(urlParameter != null)
            {
                foreach(var parameter in urlParameter.Split('_'))
                {
                    decimal priceStart = decimal.Parse(parameter.Split('-')[0]);
                    decimal priceEnd = decimal.Parse(parameter.Split('-')[1]);
                    requestInfo.PriceFilters.Single(pf => pf.PriceStart == priceStart && pf.PriceEnd == priceEnd).Selected = true;
                }
            }

            Expression<Func<Item, bool>> predicate = null;
            foreach (var priceFilter in requestInfo.PriceFilters.Where(f => f.Selected))
            {
                Expression<Func<Item, bool>> subPredicate = i => i.CurrentPrice >= priceFilter.PriceStart && i.CurrentPrice <= priceFilter.PriceEnd;
                if (predicate == null)
                {
                    predicate = subPredicate;
                }
                else
                {
                    predicate = predicate.Or(subPredicate);
                }
            }

            if (predicate != null)
            {
                requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.AsExpandable().Where(predicate);
            }
        }

        void ParseBrandsFilters(ref RequestInfo requestInfo, string brands)
        {
            var brandsString = new string[0];
            if(brands != null && brands.Trim() != "")
            {
                brandsString = brands.Split('_');
            }

            var brandList = requestInfo.ItemsRequest.GroupBy(i => i.Brand).Select(b => new {
                Brand = b.Key,
                Quantity = b.Count()
            });

            var resultList = new List<SearchBrandViewModel>();
            foreach(var brand in brandList)
            {
                var brandVM = new SearchBrandViewModel();
                brandVM.Brand = brand.Brand;
                if(brand.Brand != null)
                {
                    brandVM.Selected = brandsString.Contains(brand.Brand.LinkName);
                }
                brandVM.ItemsCount = brand.Quantity;
                resultList.Add(brandVM);
            }
            requestInfo.Brands = resultList;

            var brandsId = resultList.Where(b => b.Selected).Select(b => b.Brand.Id).ToArray();
            if(brandsId.Count() > 0)
            {
                requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.Where(i => brandsId.Contains((int)i.BrandId));
            }
        }

        void SortRequest(ref RequestInfo requestInfo, string sort)
        {
            requestInfo.Sort = sort;
            if(sort == "low_price")
            {
                requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.OrderBy(i => i.CurrentPrice).ThenBy(i => i.Name);
            }
            else if (sort == "high_price")
            {
                requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.OrderByDescending(i => i.CurrentPrice).ThenBy(i => i.Name);
            }
            else if (sort == "popularity")
            {
                requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.OrderByDescending(i => i.Popularity).ThenBy(i => i.Name);
            }
            else if (sort == "a-z")
            {
                requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.OrderBy(i => i.Name);
            }
            else if (sort == "z-a")
            {
                requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.OrderByDescending(i => i.Name);
            }
            else if (sort == "rating")
            {
                requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.OrderByDescending(i => i.AverageRating).ThenBy(i => i.Name);
            }
            else
            {
                requestInfo.ItemsRequestAfterFilter = requestInfo.ItemsRequestAfterFilter.OrderBy(i => i.Popularity).ThenBy(i => i.Name);
            }
        }

        void FinalizeRequest(ref RequestInfo requestInfo, int page)
        {
            requestInfo.PageNumber = page;
            requestInfo.PageItems = requestInfo.ItemsRequestAfterFilter.Skip(ItemPerPage * requestInfo.PageNumber).Take(ItemPerPage).ToArray();
            requestInfo.ItemCountAfterFilter = requestInfo.ItemsRequestAfterFilter.Count();
            requestInfo.PageCount = (requestInfo.ItemCountAfterFilter / ItemPerPage) + (requestInfo.ItemCountAfterFilter % ItemPerPage != 0 ? 1 : 0);
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


        CategoryViewModel[] GetCategories()
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