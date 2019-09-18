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

    public class SearchViewModel
    {
        public string Search { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public IEnumerable<SearchItemViewModel> Items { get; set; }
    }
}