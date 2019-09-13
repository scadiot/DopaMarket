using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ItemKeyword
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public int KeywordId { get; set; }
    }
}