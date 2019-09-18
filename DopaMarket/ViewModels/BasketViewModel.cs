using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class BasketItemViewModel
    {
        public Models.Item Item { get; set; }
        public int Quantity { get; set; }
    }

    public class BasketViewModel
    {
        public IEnumerable<BasketItemViewModel> Items { get; set; }
    }
}