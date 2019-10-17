using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Dto
{
    public class SearchRequest
    {
        public string Keywords { get; set; }
        public string Category { get; set; }
        public string Brands { get; set; }
        public string Sort { get; set; }
        public decimal FilterPriceMin { get; set; }
        public decimal FilterPriceMax { get; set; }
        public int Page { get; set; }
    }

    public class SearchBrand
    {
        public Brand Brand { get; set; }
        public bool Selected { get; set; }
        public int ItemsCount { get; set; }
    }

    public class SearchResult
    {
        public int ItemCount { get; set; }
        public int ItemCountAfterFilter { get; set; }
        public int PageCount { get; set; }
        public int Page { get; set; }

        public decimal PriceMin { get; set; }
        public decimal PriceMax { get; set; }

        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}