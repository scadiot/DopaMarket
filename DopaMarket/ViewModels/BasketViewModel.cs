using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class CartItemViewModel
    {
        public Models.Item Item { get; set; }
        public int Quantity { get; set; }
    }

    public class CartViewModel
    {
        public decimal SubTotal { get; set; }
        public IEnumerable<CartItemViewModel> Items { get; set; }
    }
}