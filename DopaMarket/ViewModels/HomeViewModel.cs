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
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Item> NewArrivals { get; set; }
        public IEnumerable<Item> BestSellers { get; set; }
        public IEnumerable<Item> TopRated { get; set; }
        public IEnumerable<Brand> Brands { get; set; }

        public IEnumerable<ItemPromotion> ItemPromotions { get; set; }
    }
}