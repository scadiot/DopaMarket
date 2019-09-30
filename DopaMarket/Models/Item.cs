using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Required]
        [StringLength(200)]
        [Index(IsUnique = true)]
        public string LinkName { get; set; }

        [Required]
        public string TinyDescriptive { get; set; }

        [Required]
        public string Descriptive { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal CurrentPrice { get; set; }

        public DateTime InsertDate { get; set; }

        public bool ForSale { get; set; }

        public int? BrandId { get; set; }

        public Brand Brand { get; set; }

        public int? MainCategoryId { get; set; }

        public Category MainCategory { get; set; }

        public decimal AverageRating { get; set; }

        public int Popularity { get; set; }

        public string SKU { get; set; }

        public int StockCount { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal Weight { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal Width { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal Height { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal Length { get; set; }
    }
}