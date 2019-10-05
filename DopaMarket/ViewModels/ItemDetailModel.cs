using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class ItemSpecificationModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unity { get; set; }
    }

    public class StarInfo
    {
        public int Value { get; set; }
        public int Count { get; set; }
        public decimal Ratio { get; set; }
    }

    public class ItemDetailModel
    {
        public Models.Item Item { get; set; }
        public IEnumerable<ItemSpecificationModel> ItemSpecifications { get; set; }
        public IEnumerable<Models.ItemReview> Reviews { get; set; }
        public IEnumerable<Models.ItemImage> Images { get; set; }
        public IEnumerable<Models.ItemFeature> Features { get; set; }

        public IEnumerable<StarInfo> Stars { get; set; }
    }
}