using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class SearchBrandViewModel
    {
        public Brand Brand { get; set; }
        public bool Selected { get; set; }
        public int ItemsCount { get; set; }
    }

    public class SearchViewModel
    {
        public string Query { get; set; }
        public string Category { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public string Sort { get; set; }

        public decimal PriceMin { get; set; }
        public decimal PriceMax { get; set; }
        public decimal PriceFilterMin { get; set; }
        public decimal PriceFilterMax { get; set; }

        public IEnumerable<SearchBrandViewModel> Brands { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public IEnumerable<Category> ChildrenCategories { get; set; }

        public IEnumerable<Item> Items { get; set; }
    }
}