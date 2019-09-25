using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ItemFeature
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        [Required]
        public string Text { get; set; }
    }
}