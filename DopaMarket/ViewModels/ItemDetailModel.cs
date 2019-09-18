using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class ItemDetailItemInfoModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unity { get; set; }
    }

    public class ItemDetailModel
    {
        public Models.Item Item { get; set; }
        public ItemDetailItemInfoModel[] ItemInfos { get; set; }
    }
}