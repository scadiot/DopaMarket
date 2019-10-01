using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ItemLink
    {
        public int Id { get; set; }
        public int MainItemId { get; set; }
        public Item MainItem { get; set; }
        public int OtherItemId { get; set; }
        public Item OtherItem { get; set; }
    }
}