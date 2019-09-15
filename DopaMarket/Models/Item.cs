using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [AllowHtml]
        [Required]
        public string LinkName { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal CurrentPrice { get; set; }

        public int StockCount { get; set; }

        public DateTime InsertDate { get; set; }

        public bool OnSale { get; set; }
    }
}