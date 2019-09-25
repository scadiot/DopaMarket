using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public DateTime Date { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal ItemsSumPrice { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal ExpeditionPrice { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal TotalPrice { get; set; }
    }
}