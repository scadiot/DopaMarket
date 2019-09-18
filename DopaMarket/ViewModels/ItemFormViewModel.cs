using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.ViewModels
{
    public class ItemFormViewModel
    {
        public Models.Item Item { get; set; }
        public int[] SelectedCategoryIds { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public string Keywords { get; set; }
        public HttpPostedFileBase[] UploadImages { get; set; }
        public IEnumerable<ItemImage> Images { get; set; }

        [DataType(DataType.MultilineText)]
        public string ItemInfosData { get; set; }
    }
}