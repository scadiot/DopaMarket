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
        public IEnumerable<SearchItemViewModel> NewArrivals { get; set; }
        public IEnumerable<SearchItemViewModel> BestSellers { get; set; }
        public IEnumerable<SearchItemViewModel> TopRated { get; set; }
        public IEnumerable<Brand> Brands { get; set; }

        public IEnumerable<ItemPromotion> ItemPromotions { get; set; }
    }
}