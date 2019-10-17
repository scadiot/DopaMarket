using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class CheckoutItemViewModel
    {
        public Models.Item Item { get; set; }
        public int Count { get; set; }
    }
    public class CheckoutViewModel
    {
        public Models.OrderAddress BillingAddress { get; set; }
        public Models.OrderAddress ShippingAddress { get; set; }
        public IEnumerable<CheckoutItemViewModel> Items { get; set; }
    }
}