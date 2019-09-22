using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.ViewModels
{
    public class CategoryFormViewModel
    {
        public Models.Category Category { get; set; }
        public IEnumerable<Models.Item> Items { get; set; }
        public IEnumerable<Models.Category> Breadcrumb { get; set; }
    }
}