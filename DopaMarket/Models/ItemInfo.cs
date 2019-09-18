using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ItemInfo
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int ItemInfoTypeId { get; set; }
        public ItemInfoType ItemInfoType { get; set; }
        public int IntegerValue { get; set; }
        public decimal DecimalValue { get; set; } 
        public bool BooleanValue { get; set; }
        public string StringValue { get; set; }
    }
}