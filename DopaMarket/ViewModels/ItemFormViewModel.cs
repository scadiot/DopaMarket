using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.ViewModels
{
    public class ItemFormViewModel
    {
        public Models.Item Item { get; set; }
        public int[] SelectedCategoryIds { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public string Keywords { get; set; }
        public HttpPostedFileBase[] UploadImage { get; set; }
    }
}