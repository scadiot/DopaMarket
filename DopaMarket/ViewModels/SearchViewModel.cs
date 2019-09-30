using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class SearchItemViewModel
    {
        public Item Item { get; set; }
        public ItemImage Image { get; set; }
    }

    public class SearchBrandViewModel
    {
        public Brand Brand { get; set; }
        public bool Selected { get; set; }
        public int ItemsCount { get; set; }
    }

    public class PriceFilter
    {
        public decimal PriceStart { get; set; }
        public decimal PriceEnd { get; set; }
        public int Count { get; set; }
        public bool Selected { get; set; }
    }

    public class SearchViewModel
    {
        public string Query { get; set; }
        public string Category { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public string Sort { get; set; }

        public decimal PriceRangeStart { get; set; }
        public decimal PriceRangeEnd { get; set; }
        public decimal PriceRangeMin { get; set; }
        public decimal PriceRangeMax { get; set; }

        public IEnumerable<PriceFilter> PriceFilters { get; set; }

        public IEnumerable<SearchBrandViewModel> Brands { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public IEnumerable<Category> ChildrenCategories { get; set; }

        public IEnumerable<SearchItemViewModel> Items { get; set; }
    }
}