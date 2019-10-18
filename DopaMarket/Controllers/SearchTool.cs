using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Controllers
{
    public class SearchTool
    {
        public class SearchRequest
        {
            public int Page { get; set; }
            public int ItemPerPage { get; set; }
            public Category Category { get; set; }
            public string[] Keywords { get; set; }
            public string[] Brands { get; set; }
            public string Sort { get; set; }
            public decimal FilterPriceMin { get; set; }
            public decimal FilterPriceMax { get; set; }
        }

        public class SearchResult
        {
            public int ItemCount { get; set; }
            public int ItemCountAfterFilter { get; set; }
            public int PageCount { get; set; }

            public decimal PriceMin { get; set; }
            public decimal PriceMax { get; set; }

            public IEnumerable<Brand> Brands { get; set; }
            public IEnumerable<Item> Items { get; set; }
            public IEnumerable<Category> Categories { get; set; }
        }

        ApplicationDbContext _context;

        SearchRequest _searchRequest;
        IQueryable<Item> _itemsRequest;
        IQueryable<Item> _itemsRequestAfterFilter;
        SearchResult _result;

        public SearchTool()
        {
            _context = new ApplicationDbContext();
        }

        public SearchTool(ApplicationDbContext context)
        {
            _context = context;
        }

        public SearchResult Execute(SearchRequest searchRequest)
        {
            _searchRequest = searchRequest;

            _result = new SearchResult();
            if((_searchRequest.Keywords == null || _searchRequest.Keywords.Count() == 0) &&
               _searchRequest.Category == null)
            {
                return _result;
            }

            _itemsRequest = _context.Items;
            CreateRequestByKeywords();
            CreateRequestByCategory();
            _result.ItemCount = _itemsRequest.Count();
            GetBrands();
            _itemsRequestAfterFilter = _itemsRequest;

            FilterByPrice();
            FilterByBrand();
            SortRequest();
            FinalizeRequest();

            return _result;
        }

        void CreateRequestByKeywords()
        {
            if (_searchRequest.Keywords == null || 
                _searchRequest.Keywords.Count() == 0)
            {
                return;
            }

            var keywords = _searchRequest.Keywords;
            _itemsRequest = from item in _itemsRequest
                            join itemKeyword in _context.ItemKeywords on item.Id equals itemKeyword.ItemId
                            join keyword in _context.Keywords on itemKeyword.KeywordId equals keyword.Id
                            where keywords.Contains(keyword.Word)
                            group item by item into g
                            where g.Count() == keywords.Count()
                            select g.Key;
        }

        void CreateRequestByCategory()
        {
            if (_searchRequest.Category == null)
            {
                return;
            }

            _itemsRequest = from item in _itemsRequest
                            join ItemCategory in _context.ItemCategories on item.Id equals ItemCategory.ItemId
                            where ItemCategory.CategoryId == _searchRequest.Category.Id
                            select item;
        }

        void GetBrands()
        {
            _result.Brands = (from brand in _context.Brands
                              join item in _itemsRequest on brand.Id equals item.BrandId
                              group brand by brand into g
                              select g.Key).ToArray();

        }

        void FilterByPrice()
        {
            if(_searchRequest.FilterPriceMin != 0)
            {
                _itemsRequestAfterFilter = from item in _itemsRequestAfterFilter
                                           where item.CurrentPrice >= _searchRequest.FilterPriceMin
                                           select item;
            }

            if (_searchRequest.FilterPriceMax != 0)
            {
                _itemsRequestAfterFilter = from item in _itemsRequestAfterFilter
                                           where item.CurrentPrice <= _searchRequest.FilterPriceMax
                                           select item;
            }
        }

        void FilterByBrand()
        {
            if (_searchRequest.Brands == null || _searchRequest.Brands.Count() == 0)
            {
                return;
            }
            var brandIds = _context.Brands.Where(b => _searchRequest.Brands.Contains(b.LinkName)).Select(b => (int?)b.Id).ToArray();
            _itemsRequestAfterFilter = from item in _itemsRequestAfterFilter
                                       where brandIds.Contains(item.BrandId)
                                       select item;
        }

        void SortRequest()
        {
            if (_searchRequest.Sort == "low_price")
            {
                _itemsRequestAfterFilter = _itemsRequestAfterFilter.OrderBy(i => i.CurrentPrice).ThenBy(i => i.Name);
            }
            else if (_searchRequest.Sort == "high_price")
            {
                _itemsRequestAfterFilter = _itemsRequestAfterFilter.OrderByDescending(i => i.CurrentPrice).ThenBy(i => i.Name);
            }
            else if (_searchRequest.Sort == "popularity")
            {
                _itemsRequestAfterFilter = _itemsRequestAfterFilter.OrderByDescending(i => i.Popularity).ThenBy(i => i.Name);
            }
            else if (_searchRequest.Sort == "a-z")
            {
                _itemsRequestAfterFilter = _itemsRequestAfterFilter.OrderBy(i => i.Name);
            }
            else if (_searchRequest.Sort == "z-a")
            {
                _itemsRequestAfterFilter = _itemsRequestAfterFilter.OrderByDescending(i => i.Name);
            }
            else if (_searchRequest.Sort == "rating")
            {
                _itemsRequestAfterFilter = _itemsRequestAfterFilter.OrderByDescending(i => i.AverageRating).ThenBy(i => i.Name);
            }
            else
            {
                _itemsRequestAfterFilter = _itemsRequestAfterFilter.OrderBy(i => i.Popularity).ThenBy(i => i.Name);
            }
        }

        void FinalizeRequest()
        {
            _result.Items = _itemsRequestAfterFilter.Skip(_searchRequest.ItemPerPage * _searchRequest.Page).Take(_searchRequest.ItemPerPage).ToArray();
            _result.ItemCountAfterFilter = _itemsRequestAfterFilter.Count();
            _result.PageCount = (_result.ItemCountAfterFilter / _searchRequest.ItemPerPage) + (_result.ItemCountAfterFilter % _searchRequest.ItemPerPage != 0 ? 1 : 0);
            _result.PriceMin = _itemsRequest.Min(i => i.CurrentPrice);
            _result.PriceMax = _itemsRequest.Max(i => i.CurrentPrice);
        }
    }
}