using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ItemCategory
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public int CategoryId { get; set; }
    }
}