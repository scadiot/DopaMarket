using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public enum ItemInfoValueType
    {
        Boolean,
        Interger,
        String,
        Decimal
    }

    public class ItemInfoType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ItemInfoValueType Type { get; set; }
        public string Unity { get; set; }
        public string LongName { get; set; }
    }
}