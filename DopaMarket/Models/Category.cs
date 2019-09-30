using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        [Index(IsUnique = true)]
        public string LinkName { get; set; }

        public int? ParentCategoryId { get; set; }

        public Category Parent { get; set; }

        public int ItemsCount { get; set; }
    }
}