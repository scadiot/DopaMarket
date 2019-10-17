using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DopaMarket.Models
{
    public enum OrderStatus
    {
        WaitProcess,
        Processing,
        WaitQualityCheck,
        QualityChecking,
        WaitDispatching,
        Dispatched,
        Canceled,
        Delayed,
        Delivered
    }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public DateTime Date { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal ItemsSumPrice { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal ExpeditionPrice { get; set; }

        [DataType("decimal(16 ,3")]
        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public int BillingAddressId { get; set; }

        public OrderAddress BillingAddress { get; set; }

        public int ShippingAddressId { get; set; }

        public OrderAddress ShippingAddress { get; set; }
    }
}