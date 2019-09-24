using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<SearchItemViewModel> Items { get; set; }
    }
}