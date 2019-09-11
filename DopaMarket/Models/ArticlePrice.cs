﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class ArticlePrice
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal CurrentPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}