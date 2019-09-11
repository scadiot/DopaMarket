using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ArticleCategory
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public int CategoryId { get; set; }
    }
}