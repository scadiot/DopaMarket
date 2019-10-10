using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ItemCompare
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public string SessionId { get; set; }
    }
}